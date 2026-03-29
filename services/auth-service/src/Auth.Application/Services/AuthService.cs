using System.Threading.Tasks;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Application.Entities;

namespace Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    //Veritabani arayuzumuzu(IUserRepository) iceri aliyoruz
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<AuthResult> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        //1.Kullanici adi daha once alinmis mi kontrol et
       var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            return new AuthResult { Success = false }; // kullanici zaten var!
        }

        //2.Yeni kullanici olustur(sifre hashlenebilir)
        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = request.Password // sifre
        };

        //3.veritabanina kaydet
       await _userRepository.CreateAsync(newUser);

        //4.basarili sonuc don
        return new AuthResult
        {
            Success = true,
            Data = new UserData { Username = newUser.Username }
        };
    }

    public Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        throw new NotImplementedException();
    }
}