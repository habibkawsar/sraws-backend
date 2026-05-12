using SponsorshipApproval.Api.Application.DTOs;

namespace SponsorshipApproval.Api.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
}
