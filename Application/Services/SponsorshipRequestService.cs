using Microsoft.EntityFrameworkCore;
using SponsorshipApproval.Api.Application.DTOs;
using SponsorshipApproval.Api.Application.Interfaces;
using SponsorshipApproval.Api.Domain.Entities;
using SponsorshipApproval.Api.Domain.Enums;
using SponsorshipApproval.Api.Infrastructure.Data;

namespace SponsorshipApproval.Api.Application.Services;

public sealed class SponsorshipRequestService(ApplicationDbContext dbContext) : ISponsorshipRequestService
{
    public async Task<SponsorshipRequestDetailDto> CreateDraftAsync(SponsorshipRequestCreateDto dto, int userId, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId, cancellationToken);
        await EnsureActiveSponsorshipTypeAsync(dto.SponsorshipTypeId, cancellationToken);
        ValidateFutureEventDate(dto.EventDate);

        var request = new SponsorshipRequest
        {
            RequestTitle = dto.RequestTitle.Trim(),
            RequestorName = user.FullName,
            Department = dto.Department.Trim(),
            SponsorshipTypeId = dto.SponsorshipTypeId,
            EventOrOrganisationName = dto.EventOrOrganisationName.Trim(),
            EventDate = NormalizeDate(dto.EventDate),
            RequestedAmount = dto.RequestedAmount,
            PurposeJustification = dto.PurposeJustification.Trim(),
            ExpectedBusinessBenefit = dto.ExpectedBusinessBenefit.Trim(),
            Remarks = dto.Remarks?.Trim(),
            Status = WorkflowStatus.Draft,
            RequestorId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.SponsorshipRequests.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);
        await AddHistoryAsync(request, null, WorkflowStatus.Draft, WorkflowAction.Created, "Draft created", userId, cancellationToken);
        return await GetDetailInternalAsync(request.Id, cancellationToken);
    }

    public async Task<SponsorshipRequestDetailDto> UpdateDraftAsync(int id, SponsorshipRequestUpdateDto dto, int userId, CancellationToken cancellationToken)
    {
        var request = await GetRequestForMutationAsync(id, cancellationToken);
        EnsureOwner(request, userId);
        EnsureStatus(request, WorkflowStatus.Draft);
        await EnsureActiveSponsorshipTypeAsync(dto.SponsorshipTypeId, cancellationToken);
        ValidateFutureEventDate(dto.EventDate);

        request.RequestTitle = dto.RequestTitle.Trim();
        request.Department = dto.Department.Trim();
        request.SponsorshipTypeId = dto.SponsorshipTypeId;
        request.EventOrOrganisationName = dto.EventOrOrganisationName.Trim();
        request.EventDate = NormalizeDate(dto.EventDate);
        request.RequestedAmount = dto.RequestedAmount;
        request.PurposeJustification = dto.PurposeJustification.Trim();
        request.ExpectedBusinessBenefit = dto.ExpectedBusinessBenefit.Trim();
        request.Remarks = dto.Remarks?.Trim();
        request.UpdatedAt = DateTime.UtcNow;
        await AddHistoryAsync(request, WorkflowStatus.Draft, WorkflowStatus.Draft, WorkflowAction.SavedDraft, "Draft updated", userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetDetailInternalAsync(id, cancellationToken);
    }

    public async Task<SponsorshipRequestDetailDto> SubmitAsync(int id, int userId, CancellationToken cancellationToken)
    {
        var request = await GetRequestForMutationAsync(id, cancellationToken);
        EnsureOwner(request, userId);
        EnsureStatus(request, WorkflowStatus.Draft);
        ChangeStatus(request, WorkflowStatus.PendingManagerApproval);
        await AddHistoryAsync(request, WorkflowStatus.Draft, request.Status, WorkflowAction.Submitted, "Submitted for manager approval", userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetDetailInternalAsync(id, cancellationToken);
    }

    public async Task<SponsorshipRequestDetailDto> CancelAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken)
    {
        var request = await GetRequestForMutationAsync(id, cancellationToken);
        EnsureOwner(request, userId);
        if (request.Status is not (WorkflowStatus.Draft or WorkflowStatus.PendingManagerApproval))
        {
            throw new InvalidOperationException("Only draft or pending manager approval requests can be cancelled.");
        }

        var from = request.Status;
        ChangeStatus(request, WorkflowStatus.Cancelled);
        await AddHistoryAsync(request, from, request.Status, WorkflowAction.Cancelled, dto.Remarks ?? "Cancelled by requestor", userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetDetailInternalAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SponsorshipRequestListDto>> GetOwnAsync(int userId, CancellationToken cancellationToken) =>
        await BaseListQuery().Where(x => x.RequestorId == userId).OrderByDescending(x => x.UpdatedAt).Select(x => ToListDto(x)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<SponsorshipRequestListDto>> GetPendingManagerAsync(CancellationToken cancellationToken) =>
        await BaseListQuery().Where(x => x.Status == WorkflowStatus.PendingManagerApproval).OrderBy(x => x.CreatedAt).Select(x => ToListDto(x)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<SponsorshipRequestListDto>> GetPendingFinanceAsync(CancellationToken cancellationToken) =>
        await BaseListQuery().Where(x => x.Status == WorkflowStatus.PendingFinanceReview).OrderBy(x => x.CreatedAt).Select(x => ToListDto(x)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<SponsorshipRequestListDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await BaseListQuery().OrderByDescending(x => x.UpdatedAt).Select(x => ToListDto(x)).ToListAsync(cancellationToken);

    public async Task<SponsorshipRequestDetailDto> GetByIdAsync(int id, int userId, string role, CancellationToken cancellationToken)
    {
        var request = await BaseDetailQuery().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new KeyNotFoundException("Sponsorship request was not found.");
        var canView = role switch
        {
            "Requestor" => request.RequestorId == userId,
            "Manager" => request.Status == WorkflowStatus.PendingManagerApproval,
            "FinanceAdmin" => request.Status == WorkflowStatus.PendingFinanceReview,
            "SystemAdmin" => true,
            _ => false
        };

        if (!canView)
        {
            throw new UnauthorizedAccessException("You are not authorized to view this request.");
        }

        return ToDetailDto(request);
    }

    public Task<SponsorshipRequestDetailDto> ManagerApproveAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken) =>
        TransitionAsync(id, WorkflowStatus.PendingManagerApproval, WorkflowStatus.PendingFinanceReview, WorkflowAction.ManagerApproved, dto.Remarks ?? "Approved by manager", userId, cancellationToken);

    public Task<SponsorshipRequestDetailDto> ManagerRejectAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken) =>
        TransitionAsync(id, WorkflowStatus.PendingManagerApproval, WorkflowStatus.Rejected, WorkflowAction.ManagerRejected, RequireRemarks(dto), userId, cancellationToken);

    public Task<SponsorshipRequestDetailDto> FinanceApproveAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken) =>
        TransitionAsync(id, WorkflowStatus.PendingFinanceReview, WorkflowStatus.Approved, WorkflowAction.FinanceApproved, dto.Remarks ?? "Final approved by finance", userId, cancellationToken);

    public Task<SponsorshipRequestDetailDto> FinanceRejectAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken) =>
        TransitionAsync(id, WorkflowStatus.PendingFinanceReview, WorkflowStatus.Rejected, WorkflowAction.FinanceRejected, RequireRemarks(dto), userId, cancellationToken);

    private async Task<SponsorshipRequestDetailDto> TransitionAsync(int id, WorkflowStatus expected, WorkflowStatus target, WorkflowAction action, string remarks, int userId, CancellationToken cancellationToken)
    {
        var request = await GetRequestForMutationAsync(id, cancellationToken);
        EnsureStatus(request, expected);
        ChangeStatus(request, target);
        await AddHistoryAsync(request, expected, target, action, remarks, userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetDetailInternalAsync(id, cancellationToken);
    }

    private IQueryable<SponsorshipRequest> BaseListQuery() => dbContext.SponsorshipRequests.AsNoTracking().Include(x => x.SponsorshipType);

    private IQueryable<SponsorshipRequest> BaseDetailQuery() => dbContext.SponsorshipRequests.AsNoTracking()
        .Include(x => x.SponsorshipType)
        .Include(x => x.ApprovalHistories.OrderByDescending(h => h.ActionAt)).ThenInclude(x => x.ActionByUser).ThenInclude(x => x.Role);

    private async Task<SponsorshipRequestDetailDto> GetDetailInternalAsync(int id, CancellationToken cancellationToken)
    {
        var request = await BaseDetailQuery().SingleAsync(x => x.Id == id, cancellationToken);
        return ToDetailDto(request);
    }

    private async Task<SponsorshipRequest> GetRequestForMutationAsync(int id, CancellationToken cancellationToken) =>
        await dbContext.SponsorshipRequests.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new KeyNotFoundException("Sponsorship request was not found.");

    private async Task<User> GetUserAsync(int userId, CancellationToken cancellationToken) =>
        await dbContext.Users.Include(x => x.Role).SingleOrDefaultAsync(x => x.Id == userId && x.IsActive, cancellationToken) ?? throw new UnauthorizedAccessException("User was not found or is inactive.");

    private async Task EnsureActiveSponsorshipTypeAsync(int sponsorshipTypeId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.SponsorshipTypes.AnyAsync(x => x.Id == sponsorshipTypeId && x.IsActive, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException("Selected sponsorship type is not active.");
        }
    }

    private static void EnsureOwner(SponsorshipRequest request, int userId)
    {
        if (request.RequestorId != userId)
        {
            throw new UnauthorizedAccessException("You can only modify your own requests.");
        }
    }

    private static void EnsureStatus(SponsorshipRequest request, WorkflowStatus expected)
    {
        if (request.Status != expected)
        {
            throw new InvalidOperationException($"Request must be in {expected} status. Current status is {request.Status}.");
        }
    }

    private static void ValidateFutureEventDate(DateTime eventDate)
    {
        if (eventDate.Date < DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException("Event date cannot be in the past.");
        }
    }

    private static DateTime NormalizeDate(DateTime value) => DateTime.SpecifyKind(value.Date, DateTimeKind.Utc);

    private static void ChangeStatus(SponsorshipRequest request, WorkflowStatus target)
    {
        request.Status = target;
        request.UpdatedAt = DateTime.UtcNow;
    }

    private async Task AddHistoryAsync(SponsorshipRequest request, WorkflowStatus? from, WorkflowStatus to, WorkflowAction action, string? remarks, int userId, CancellationToken cancellationToken)
    {
        await GetUserAsync(userId, cancellationToken);
        dbContext.ApprovalHistories.Add(new ApprovalHistory
        {
            SponsorshipRequestId = request.Id,
            FromStatus = from,
            ToStatus = to,
            Action = action,
            Remarks = remarks,
            ActionByUserId = userId,
            ActionAt = DateTime.UtcNow
        });
    }

    private static string RequireRemarks(WorkflowDecisionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Remarks))
        {
            throw new InvalidOperationException("Remarks are required when rejecting a request.");
        }

        return dto.Remarks.Trim();
    }

    private static SponsorshipRequestListDto ToListDto(SponsorshipRequest request) => new(
        request.Id,
        request.RequestTitle,
        request.RequestorName,
        request.Department,
        request.SponsorshipType.Name,
        request.EventOrOrganisationName,
        request.EventDate,
        request.RequestedAmount,
        request.Status,
        request.CreatedAt,
        request.UpdatedAt);

    private static SponsorshipRequestDetailDto ToDetailDto(SponsorshipRequest request) => new(
        request.Id,
        request.RequestTitle,
        request.RequestorName,
        request.Department,
        request.SponsorshipTypeId,
        request.SponsorshipType.Name,
        request.EventOrOrganisationName,
        request.EventDate,
        request.RequestedAmount,
        request.PurposeJustification,
        request.ExpectedBusinessBenefit,
        request.Remarks,
        request.Status,
        request.CreatedAt,
        request.UpdatedAt,
        request.ApprovalHistories.Select(h => new ApprovalHistoryDto(h.Id, h.FromStatus, h.ToStatus, h.Action, h.Remarks, h.ActionByUser.FullName, h.ActionByUser.Role.Name, h.ActionAt)).ToList());
}
