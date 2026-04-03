using System.Threading.Tasks;
using Auth.Application.DTOs; 

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<TokenValidationResult> ValidateTokenAsync(string token);
    Task<UserData?> GetUserAsync(string username);
    Task<bool> LogoutAsync(string token);
}
