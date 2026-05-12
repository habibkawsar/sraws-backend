using System.ComponentModel.DataAnnotations;
using SponsorshipApproval.Api.Domain.Enums;

namespace SponsorshipApproval.Api.Application.DTOs;

public sealed record SponsorshipTypeDto(int Id, string Name, string? Description, bool IsActive);

public sealed class SponsorshipTypeCreateDto
{
    [Required, MaxLength(120)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; init; }

    public bool IsActive { get; init; } = true;
}

public sealed class SponsorshipTypeUpdateDto
{
    [Required, MaxLength(120)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; init; }

    public bool IsActive { get; init; }
}

public sealed record ApprovalHistoryDto(
    int Id,
    WorkflowStatus? FromStatus,
    WorkflowStatus ToStatus,
    WorkflowAction Action,
    string? Remarks,
    string ActionBy,
    string ActionByRole,
    DateTime ActionAt);
