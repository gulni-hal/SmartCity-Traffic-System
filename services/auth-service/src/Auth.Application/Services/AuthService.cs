using System.Threading.Tasks;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;

namespace Auth.Application.Services;



public class AuthService : IAuthService
{
    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var result = new AuthResult
        {
            Success = true,
            Data = new UserData
            {
                Username = request.Username
            }
        };

        return await Task.FromResult(result);
    }
}