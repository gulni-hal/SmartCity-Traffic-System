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
    private readonly List<FineRecord> _fines = new();

    public Task CreateAsync(FineRecord record)
    {
        _fines.Add(record);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<FineRecord>> GetByLicensePlateAsync(string licensePlate)
    {
        var result = _fines.Where(f => f.LicensePlate == licensePlate);
        return Task.FromResult(result.AsEnumerable());
    }

    public Task<IEnumerable<FineRecord>> GetAllAsync()
    {
        return Task.FromResult(_fines.AsEnumerable());
    }

    public Task<bool> DeleteAsync(string id)
    {
        var removed = _fines.RemoveAll(f => f.Id == id) > 0;
        return Task.FromResult(removed);
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
    [Fact]

    public async Task GetFinesByPlate_Should_Return_Records_When_Plate_Exists()
    {
        // Arrange
        var fakeRepo = new FakeFineRepository();
        await fakeRepo.CreateAsync(new FineRecord { LicensePlate = "34ABC123", Amount = 500, Reason = "Hız" });
        var service = new FineService(fakeRepo);

        // Act
        var result = await service.GetFinesByPlateAsync("34ABC123");

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal("34ABC123", result.First().LicensePlate);
    }

    [Fact]
    public async Task GetFinesByPlate_Should_Return_Empty_When_Plate_Does_Not_Exist()
    {
        var fakeRepo = new FakeFineRepository();
        var service = new FineService(fakeRepo);

        var result = await service.GetFinesByPlateAsync("99BOS999");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllFines_Should_Return_All_Data()
    {
        var fakeRepo = new FakeFineRepository();
        await fakeRepo.CreateAsync(new FineRecord { LicensePlate = "34ABC123", Amount = 500, Reason = "Hız" });
        await fakeRepo.CreateAsync(new FineRecord { LicensePlate = "06XYZ789", Amount = 750, Reason = "Park" });

        var service = new FineService(fakeRepo);

        var result = await service.GetAllFinesAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task DeleteFine_Should_Return_True_When_Record_Exists()
    {
        var fakeRepo = new FakeFineRepository();
        var record = new FineRecord { LicensePlate = "34ABC123", Amount = 500, Reason = "Hız" };
        await fakeRepo.CreateAsync(record);

        var service = new FineService(fakeRepo);

        var result = await service.DeleteFineAsync(record.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteFine_Should_Return_False_When_Record_Not_Found()
    {
        var fakeRepo = new FakeFineRepository();
        var service = new FineService(fakeRepo);

        var result = await service.DeleteFineAsync("not-found-id");

        Assert.False(result);
    }

}
