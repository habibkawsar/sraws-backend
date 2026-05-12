using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipApproval.Api.Application.DTOs;
using SponsorshipApproval.Api.Application.Interfaces;

namespace SponsorshipApproval.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/requests")]
public sealed class RequestsController(ISponsorshipRequestService requestService) : ControllerBase
{
    [HttpGet("mine")]
    [Authorize(Roles = "Requestor")]
    public async Task<ActionResult<IReadOnlyCollection<SponsorshipRequestListDto>>> Mine(CancellationToken cancellationToken) =>
        Ok(await requestService.GetOwnAsync(User.GetUserId(), cancellationToken));

    [HttpGet("pending-manager")]
    [Authorize(Roles = "Manager,SystemAdmin")]
    public async Task<ActionResult<IReadOnlyCollection<SponsorshipRequestListDto>>> PendingManager(CancellationToken cancellationToken) =>
        Ok(await requestService.GetPendingManagerAsync(cancellationToken));

    [HttpGet("pending-finance")]
    [Authorize(Roles = "FinanceAdmin,SystemAdmin")]
    public async Task<ActionResult<IReadOnlyCollection<SponsorshipRequestListDto>>> PendingFinance(CancellationToken cancellationToken) =>
        Ok(await requestService.GetPendingFinanceAsync(cancellationToken));

    [HttpGet("all")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<ActionResult<IReadOnlyCollection<SponsorshipRequestListDto>>> All(CancellationToken cancellationToken) =>
        Ok(await requestService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await requestService.GetByIdAsync(id, User.GetUserId(), User.GetRole(), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Requestor")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> CreateDraft(SponsorshipRequestCreateDto request, CancellationToken cancellationToken)
    {
        var created = await requestService.CreateDraftAsync(request, User.GetUserId(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Requestor")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> UpdateDraft(int id, SponsorshipRequestUpdateDto request, CancellationToken cancellationToken) =>
        Ok(await requestService.UpdateDraftAsync(id, request, User.GetUserId(), cancellationToken));

    [HttpPost("{id:int}/submit")]
    [Authorize(Roles = "Requestor")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> Submit(int id, CancellationToken cancellationToken) =>
        Ok(await requestService.SubmitAsync(id, User.GetUserId(), cancellationToken));

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = "Requestor")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> Cancel(int id, WorkflowDecisionDto request, CancellationToken cancellationToken) =>
        Ok(await requestService.CancelAsync(id, request, User.GetUserId(), cancellationToken));

    [HttpPost("{id:int}/manager/approve")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> ManagerApprove(int id, WorkflowDecisionDto request, CancellationToken cancellationToken) =>
        Ok(await requestService.ManagerApproveAsync(id, request, User.GetUserId(), cancellationToken));

    [HttpPost("{id:int}/manager/reject")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> ManagerReject(int id, WorkflowDecisionDto request, CancellationToken cancellationToken) =>
        Ok(await requestService.ManagerRejectAsync(id, request, User.GetUserId(), cancellationToken));

    [HttpPost("{id:int}/finance/approve")]
    [Authorize(Roles = "FinanceAdmin")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> FinanceApprove(int id, WorkflowDecisionDto request, CancellationToken cancellationToken) =>
        Ok(await requestService.FinanceApproveAsync(id, request, User.GetUserId(), cancellationToken));

    [HttpPost("{id:int}/finance/reject")]
    [Authorize(Roles = "FinanceAdmin")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> FinanceReject(int id, WorkflowDecisionDto request, CancellationToken cancellationToken) =>
        Ok(await requestService.FinanceRejectAsync(id, request, User.GetUserId(), cancellationToken));
}
