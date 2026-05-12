using SponsorshipApproval.Api.Application.DTOs;

namespace SponsorshipApproval.Api.Application.Interfaces;

public interface ISponsorshipRequestService
{
    Task<SponsorshipRequestDetailDto> CreateDraftAsync(SponsorshipRequestCreateDto dto, int userId, CancellationToken cancellationToken);
    Task<SponsorshipRequestDetailDto> UpdateDraftAsync(int id, SponsorshipRequestUpdateDto dto, int userId, CancellationToken cancellationToken);
    Task<SponsorshipRequestDetailDto> SubmitAsync(int id, int userId, CancellationToken cancellationToken);
    Task<SponsorshipRequestDetailDto> CancelAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SponsorshipRequestListDto>> GetOwnAsync(int userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SponsorshipRequestListDto>> GetPendingManagerAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SponsorshipRequestListDto>> GetPendingFinanceAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SponsorshipRequestListDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<SponsorshipRequestDetailDto> GetByIdAsync(int id, int userId, string role, CancellationToken cancellationToken);
    Task<SponsorshipRequestDetailDto> ManagerApproveAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken);
    Task<SponsorshipRequestDetailDto> ManagerRejectAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken);
    Task<SponsorshipRequestDetailDto> FinanceApproveAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken);
    Task<SponsorshipRequestDetailDto> FinanceRejectAsync(int id, WorkflowDecisionDto dto, int userId, CancellationToken cancellationToken);
}
