using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fine.Application.Services;
using Fine.Application.DTOs;
using Fine.Application.Interfaces;
using Fine.Application.Entities;

namespace Fine.UnitTests.Services;

// Test için sadece hafızada çalışan sahte (Fake) bir veritabanı simülasyonu
public class FakeFineRepository : IFineRepository
{
    // Sahte veritabanımız verileri geçici olarak bu listede tutacak
    private readonly List<FineRecord> _fines = new();

    public Task CreateAsync(FineRecord record)
    {
        _fines.Add(record);
        return Task.CompletedTask;
    }

    // YENİ EKLENEN METOT: Arayüzdeki sözleşmeyi yerine getiriyoruz
    public Task<IEnumerable<FineRecord>> GetByLicensePlateAsync(string licensePlate)
    {
        // Plakaya göre listede arama yapıyoruz
        var result = _fines.Where(f => f.LicensePlate == licensePlate);
        return Task.FromResult(result.AsEnumerable());
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