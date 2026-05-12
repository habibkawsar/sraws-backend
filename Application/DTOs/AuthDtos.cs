using System.ComponentModel.DataAnnotations;

namespace SponsorshipApproval.Api.Application.DTOs;

public sealed class LoginRequestDto
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(8), MaxLength(128)]
    public string Password { get; init; } = string.Empty;
}

public sealed record AuthUserDto(int Id, string FullName, string Email, string Department, string Role);

public sealed record LoginResponseDto(string AccessToken, DateTime ExpiresAtUtc, AuthUserDto User);
