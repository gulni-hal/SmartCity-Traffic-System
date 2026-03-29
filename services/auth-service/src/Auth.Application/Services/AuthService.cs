using Auth.Application.DTOs;
using Auth.Application.Entities;
using Auth.Application.Interfaces;

namespace Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser is not null)
        {
            return new AuthResult { Success = false };
        }

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = request.Password,
            Role = request.Role
        };

        await _userRepository.CreateAsync(newUser);

        return new AuthResult
        {
            Success = true,
            Data = new UserData
            {
                Username = newUser.Username,
                Role = newUser.Role
            }
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user is null || user.PasswordHash != request.Password)
        {
            return new AuthResult { Success = false };
        }

        user.Token = Guid.NewGuid().ToString();
        await _userRepository.UpdateAsync(user);

        return new AuthResult
        {
            Success = true,
            Data = new UserData
            {
                Username = user.Username,
                Role = user.Role,
                Token = user.Token
            }
        };
    }

    public async Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        var user = await _userRepository.GetByTokenAsync(token);

        if (user is null)
        {
            return new TokenValidationResult
            {
                IsValid = false
            };
        }

        return new TokenValidationResult
        {
            IsValid = true,
            Username = user.Username,
            Role = user.Role
        };
    }
}
