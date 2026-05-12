using System.ComponentModel.DataAnnotations;
using SponsorshipApproval.Api.Domain.Enums;

namespace SponsorshipApproval.Api.Application.DTOs;

public record SponsorshipRequestCreateDto
{
    [Required, MaxLength(180)]
    public string RequestTitle { get; init; } = string.Empty;

    [Required, MaxLength(120)]
    public string Department { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int SponsorshipTypeId { get; init; }

    [Required, MaxLength(180)]
    public string EventOrOrganisationName { get; init; } = string.Empty;

    [Required]
    public DateTime EventDate { get; init; }

    [Range(1, 999999999)]
    public decimal RequestedAmount { get; init; }

    [Required, MinLength(20), MaxLength(2000)]
    public string PurposeJustification { get; init; } = string.Empty;

    [Required, MinLength(20), MaxLength(2000)]
    public string ExpectedBusinessBenefit { get; init; } = string.Empty;

    [MaxLength(1000)]
    public string? Remarks { get; init; }
}

public sealed record SponsorshipRequestUpdateDto : SponsorshipRequestCreateDto;

public sealed class WorkflowDecisionDto
{
    [MaxLength(1000)]
    public string? Remarks { get; init; }
}

public sealed record SponsorshipRequestListDto(
    int Id,
    string RequestTitle,
    string RequestorName,
    string Department,
    string SponsorshipType,
    string EventOrOrganisationName,
    DateTime EventDate,
    decimal RequestedAmount,
    WorkflowStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record SponsorshipRequestDetailDto(
    int Id,
    string RequestTitle,
    string RequestorName,
    string Department,
    int SponsorshipTypeId,
    string SponsorshipType,
    string EventOrOrganisationName,
    DateTime EventDate,
    decimal RequestedAmount,
    string PurposeJustification,
    string ExpectedBusinessBenefit,
    string? Remarks,
    WorkflowStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyCollection<ApprovalHistoryDto> ApprovalHistories);
