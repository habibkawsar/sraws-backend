namespace SponsorshipApproval.Api.Domain.Enums;

public enum WorkflowStatus
{
    Draft = 1,
    PendingManagerApproval = 2,
    PendingFinanceReview = 3,
    Approved = 4,
    Rejected = 5,
    Cancelled = 6
}
