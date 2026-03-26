using System.Threading.Tasks;

namespace Auth.Application.Services;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserData
{
    public string Username { get; set; } = string.Empty;
}

public class AuthResult
{
    public bool Success { get; set; }
    public UserData? Data { get; set; }
}

public class AuthService
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