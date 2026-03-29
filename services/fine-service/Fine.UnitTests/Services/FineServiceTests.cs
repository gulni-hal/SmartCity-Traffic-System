using Xunit;
using System.Threading.Tasks;
using Fine.Application.Services;
using Fine.Application.DTOs;
using Fine.Application.Interfaces;
using Fine.Application.Entities;

namespace Fine.UnitTests.Services;

// Test için sadece hafızada çalışan sahte (Fake) bir veritabanı simülasyonu
public class FakeFineRepository : IFineRepository
{
    public Task CreateAsync(FineRecord record)
    {
        // Gerçekte bir yere kaydetmiyoruz, sadece metot hata vermeden tamamlansın
        return Task.CompletedTask;
    }
}

public class FineServiceTests
{
    [Fact]
    public async Task CreateFine_Should_Return_Success_When_Valid_Data()
    {
        // Arrange
        var fakeRepo = new FakeFineRepository(); // Sahte veritabanı
        var service = new FineService(fakeRepo); // Servise sahte DB'yi veriyoruz

        var request = new CreateFineRequest
        {
            LicensePlate = "34ABC123",
            Amount = 1500.50m,
            Reason = "Kırmızı Işık İhlali"
        };

        // Act
        var result = await service.CreateFineAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("34ABC123", result.Data.LicensePlate);
    }
}