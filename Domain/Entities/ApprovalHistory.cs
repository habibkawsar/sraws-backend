using System.ComponentModel.DataAnnotations;
using SponsorshipApproval.Api.Domain.Enums;

namespace SponsorshipApproval.Api.Domain.Entities;

public sealed class ApprovalHistory
{
    public int Id { get; set; }
    public int SponsorshipRequestId { get; set; }
    public SponsorshipRequest SponsorshipRequest { get; set; } = default!;

    public WorkflowStatus? FromStatus { get; set; }
    public WorkflowStatus ToStatus { get; set; }
    public WorkflowAction Action { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    public int ActionByUserId { get; set; }
    public User ActionByUser { get; set; } = default!;

    public DateTime ActionAt { get; set; } = DateTime.UtcNow;
}
