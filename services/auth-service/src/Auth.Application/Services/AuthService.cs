using System;
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
    public Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        
        throw new NotImplementedException();
    }
}