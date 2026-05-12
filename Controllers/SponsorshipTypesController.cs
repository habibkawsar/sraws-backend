using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipApproval.Api.Application.DTOs;
using SponsorshipApproval.Api.Application.Interfaces;

namespace SponsorshipApproval.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sponsorship-types")]
public sealed class SponsorshipTypesController(ISponsorshipTypeService sponsorshipTypeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SponsorshipTypeDto>>> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken) =>
        Ok(await sponsorshipTypeService.GetAllAsync(includeInactive && User.IsInRole("SystemAdmin"), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<ActionResult<SponsorshipTypeDto>> Create(SponsorshipTypeCreateDto request, CancellationToken cancellationToken)
    {
        var created = await sponsorshipTypeService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { includeInactive = true }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<ActionResult<SponsorshipTypeDto>> Update(int id, SponsorshipTypeUpdateDto request, CancellationToken cancellationToken) =>
        Ok(await sponsorshipTypeService.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await sponsorshipTypeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
