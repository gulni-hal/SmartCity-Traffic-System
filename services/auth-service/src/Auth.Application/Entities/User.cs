using System;

namespace Auth.Application.Entities;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "TrafficPolice";
    public string? Token { get; set; }

    // token icin son kullanma tarihi
    public DateTime? TokenExpiry { get; set; }
}