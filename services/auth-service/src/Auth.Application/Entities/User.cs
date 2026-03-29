using System;

namespace Auth.Application.Entities;

public class User
{
    // MongoDB ID'leri genelde string (ObjectId) olarak tutulur
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;


    // Gelecek role-based authorization için gerekli
    public string Role { get; set; } = "TrafficPolice";

    // Login sonrasý üretilecek basit access token
    public string? Token { get; set; }

}