using System.ComponentModel.DataAnnotations;
using SponsorshipApproval.Api.Domain.Enums;

namespace SponsorshipApproval.Api.Domain.Entities;

public sealed class SponsorshipRequest
{
    public int Id { get; set; }

    [MaxLength(180)]
    public string RequestTitle { get; set; } = string.Empty;

    [MaxLength(160)]
    public string RequestorName { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Department { get; set; } = string.Empty;

    public int SponsorshipTypeId { get; set; }
    public SponsorshipType SponsorshipType { get; set; } = default!;

    [MaxLength(180)]
    public string EventOrOrganisationName { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }

    [MaxLength(2000)]
    public string PurposeJustification { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string ExpectedBusinessBenefit { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    public WorkflowStatus Status { get; set; } = WorkflowStatus.Draft;
    public int RequestorId { get; set; }
    public User Requestor { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
}
