using SponsorshipApproval.Api.Domain.Entities;

namespace SponsorshipApproval.Api.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateToken(User user);
}
