using System;
using System.Threading.Tasks;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Application.Entities;

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
        if (existingUser != null) return new AuthResult { Success = false };

        var user = new User
        {
            Username = request.Username,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "TrafficPolice" : request.Role,
            // HOCA UYUMU: Şifreyi Düz Metin Tutmuyoruz, BCrypt ile Hashliyoruz
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _userRepository.CreateAsync(user);

        return new AuthResult { Success = true, Data = new UserData { Username = user.Username, Role = user.Role } };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        // HOCA UYUMU: Gelen şifre ile Hash'lenmiş şifreyi güvenli şekilde karşılaştırıyoruz
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResult { Success = false };
        }

        // HOCA UYUMU: Token mantığını geliştiriyoruz (1 Saatlik ömür)
        user.Token = Guid.NewGuid().ToString();
        user.TokenExpiry = DateTime.UtcNow.AddHours(1);
        await _userRepository.UpdateAsync(user);

        return new AuthResult
        {
            Success = true,
            Data = new UserData { Username = user.Username, Role = user.Role, Token = user.Token }
        };
    }

    public async Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        var user = await _userRepository.GetByTokenAsync(token);

        // HOCA UYUMU: Token var mı ve süresi dolmuş mu kontrolü
        if (user == null || user.TokenExpiry == null || user.TokenExpiry < DateTime.UtcNow)
        {
            return new TokenValidationResult { IsValid = false };
        }

        return new TokenValidationResult { IsValid = true, Username = user.Username, Role = user.Role };
    }

    public async Task<bool> LogoutAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return false;

        // HOCA UYUMU: Token'ı silerek (Revoke) çıkış yapıyoruz
        user.Token = null;
        user.TokenExpiry = null;
        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task<UserData?> GetUserAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return null;

        return new UserData
        {
            Username = user.Username,
            Role = user.Role // HOCA UYUMU: Yoruma alınan eksik rol düzeltildi
        };
    }
}