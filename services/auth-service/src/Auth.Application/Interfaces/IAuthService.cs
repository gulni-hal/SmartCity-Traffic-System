using System.Threading.Tasks;
using Auth.Application.DTOs; 

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    //Dependency Injection (DI) kurarken veya API Controller  test edilirken bu interface kullanilacakkk!!
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<TokenValidationResult> ValidateTokenAsync(string token);
    // Auth.Application/Interfaces/IAuthService.cs içerisine ekleyin:
    Task<UserData?> GetUserAsync(string username);

}