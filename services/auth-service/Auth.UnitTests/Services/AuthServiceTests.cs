using Xunit;
using Auth.Application.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task Register_Should_Create_User_When_Valid_Data()
    {
        // Arrange
        var service = new AuthService();

        var request = new RegisterRequest
        {
            Username = "testuser",
            Password = "123456"
        };

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("testuser", result.Data.Username);
    }
}