namespace Auth.Application.DTOs;
public class AuthResult
{
    public bool Success { get; set; }
    public UserData? Data { get; set; }
}