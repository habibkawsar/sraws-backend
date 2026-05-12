using Microsoft.EntityFrameworkCore;
using SponsorshipApproval.Api.Application.DTOs;
using SponsorshipApproval.Api.Application.Interfaces;
using SponsorshipApproval.Api.Infrastructure.Data;

namespace SponsorshipApproval.Api.Application.Services;

public sealed class AuthService(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.Include(x => x.Role)
            .SingleOrDefaultAsync(x => x.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (user is null || !user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = jwtTokenService.CreateToken(user);
        return new LoginResponseDto(token.Token, token.ExpiresAtUtc, new AuthUserDto(user.Id, user.FullName, user.Email, user.Department, user.Role.Name));
    }
}
