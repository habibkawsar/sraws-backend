using System.ComponentModel.DataAnnotations;

namespace SponsorshipApproval.Api.Domain.Entities;

public sealed class User
{
    public int Id { get; set; }

    [MaxLength(160)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Department { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SponsorshipRequest> SponsorshipRequests { get; set; } = new List<SponsorshipRequest>();
    public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
}
