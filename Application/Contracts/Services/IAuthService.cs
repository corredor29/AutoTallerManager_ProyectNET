using Application.DTOs.Auth;
using Application.Requests.Auth;

namespace Application.Contracts.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequest request);
}
