using System;
using Auth.Application.DTOs;
using Auth.Application.Entities;
using Auth.Application.Interfaces;
using Auth.Application.Services;
using Xunit; // Hata almamak için Xunit ekliyoruz
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.UnitTests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task Register_Should_Create_User_When_Valid_Data()
    {
        var repository = new FakeUserRepository();
        var service = new AuthService(repository);

        var request = new RegisterRequest
        {
            Username = "testuser",
            Password = "123456",
            Role = "TrafficPolice"
        };

        var result = await service.RegisterAsync(request);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("testuser", result.Data!.Username);
        Assert.Equal("TrafficPolice", result.Data.Role);
    }

    [Fact]
    public async Task Login_Should_Return_Token_When_Credentials_Are_Correct()
    {
        var repository = new FakeUserRepository();
        await repository.CreateAsync(new User
        {
            Username = "ali",
            // DÜZELTME 1: Test verisindeki şifreyi BCrypt ile hashleyerek kaydediyoruz
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "TrafficPolice"
        });

        var service = new AuthService(repository);

        var request = new LoginRequest
        {
            Username = "ali",
            Password = "123456" // Giriş yaparken düz metin yolluyoruz, servis bunu Verify ile çözecek
        };

        var result = await service.LoginAsync(request);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.False(string.IsNullOrWhiteSpace(result.Data!.Token));
        Assert.Equal("TrafficPolice", result.Data.Role);
    }

    [Fact]
    public async Task ValidateToken_Should_Return_True_When_Token_Belongs_To_User()
    {
        var repository = new FakeUserRepository();
        await repository.CreateAsync(new User
        {
            Username = "ali",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), // Tutarlılık için hashliyoruz
            Role = "TrafficPolice",
            Token = "valid-token",
            // DÜZELTME 2: Token'ın süresi dolmamış (1 saat sonrası) bir tarih veriyoruz
            TokenExpiry = DateTime.UtcNow.AddHours(1)
        });

        var service = new AuthService(repository);

        var result = await service.ValidateTokenAsync("valid-token");

        Assert.True(result.IsValid);
        Assert.Equal("ali", result.Username);
        Assert.Equal("TrafficPolice", result.Role);
    }

    [Fact]
    public async Task ValidateToken_Should_Return_False_When_Token_Is_Invalid()
    {
        var repository = new FakeUserRepository();
        var service = new AuthService(repository);

        var result = await service.ValidateTokenAsync("invalid-token");

        Assert.False(result.IsValid);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public Task CreateAsync(User user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            var user = _users.FirstOrDefault(x => x.Username == username);
            return Task.FromResult(user);
        }

        public Task<User?> GetByTokenAsync(string token)
        {
            var user = _users.FirstOrDefault(x => x.Token == token);
            return Task.FromResult(user);
        }

        public Task UpdateAsync(User user)
        {
            var existing = _users.FirstOrDefault(x => x.Id == user.Id);
            if (existing is not null)
            {
                existing.Username = user.Username;
                existing.PasswordHash = user.PasswordHash;
                existing.Role = user.Role;
                existing.Token = user.Token;
                existing.TokenExpiry = user.TokenExpiry; // Bunu da ekliyoruz
            }

            return Task.CompletedTask;
        }
    }
}