namespace Auth.Application.DTOs;
public class UserData
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Token { get; set; }

}