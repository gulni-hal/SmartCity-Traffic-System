using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffic.Application.Services;
using Traffic.Application.Interfaces;
using Traffic.Application.Entities;
using Traffic.Application.DTOs;

namespace Traffic.UnitTests.Services;

// Sahte veritabanımızı arayüze uygun hale getirdik
public class FakeTrafficRepository : ITrafficRepository
{
    private readonly List<TrafficRecord> _records = new();

    public Task CreateAsync(TrafficRecord record)
    {
        _records.Add(record);
        return Task.CompletedTask;
    }

    // YENİ EKLENEN: Arayüz sözleşmesini yerine getiriyoruz
    public Task<IEnumerable<TrafficRecord>> GetByLocationIdAsync(string locationId)
    {
        var result = _records.Where(r => r.LocationId == locationId);
        return Task.FromResult(result.AsEnumerable());
    }
    public Task<IEnumerable<TrafficRecord>> GetHotspotsAsync()
    {
        var result = _records.Where(r => r.DensityLevel == "High");
        return Task.FromResult(result.AsEnumerable());
    }
}

public class TrafficServiceTests
{
    [Fact]
    public async Task RecordTraffic_Should_Return_Success_When_Valid_Data()
    {
        var fakeRepo = new FakeTrafficRepository();
        var service = new TrafficService(fakeRepo);
        var request = new TrafficRecordRequest
        {
            LocationId = "Kadikoy-Merkez-1",
            VehicleCount = 120,
            DensityLevel = "High"
        };

        var result = await service.RecordTrafficAsync(request);

        Assert.True(result.Success);
        Assert.Equal("Trafik verisi basariyla kaydedildi.", result.Message);
    }

    [Fact]
    public async Task GetTrafficByLocation_Should_Return_Data_When_Exists()
    {
        var fakeRepo = new FakeTrafficRepository();
        await fakeRepo.CreateAsync(new TrafficRecord { LocationId = "Kadikoy", VehicleCount = 100, DensityLevel = "High" });
        var service = new TrafficService(fakeRepo);

        var result = await service.GetTrafficByLocationAsync("Kadikoy");

        Assert.NotEmpty(result);
        Assert.Equal("Kadikoy", result.First().LocationId);
    }

    [Fact]
    public async Task RecordTraffic_Should_Fail_When_VehicleCount_Is_Negative()
    {
        var fakeRepo = new FakeTrafficRepository();
        var service = new TrafficService(fakeRepo);
        var request = new TrafficRecordRequest { LocationId = "Kadikoy", VehicleCount = -5, DensityLevel = "High" };

        var result = await service.RecordTrafficAsync(request);

        Assert.False(result.Success);
        Assert.Equal("Araç sayısı negatif olamaz.", result.ErrorMessage);
    }

    [Fact]
    public async Task RecordTraffic_Should_Fail_When_LocationId_Is_Empty()
    {
        var fakeRepo = new FakeTrafficRepository();
        var service = new TrafficService(fakeRepo);

        var request = new TrafficRecordRequest
        {
            LocationId = "",
            VehicleCount = 100,
            DensityLevel = "High"
        };

        var result = await service.RecordTrafficAsync(request);

        Assert.False(result.Success);
        Assert.Equal("Lokasyon bilgisi boş olamaz.", result.ErrorMessage);
    }

    [Fact]
    public async Task RecordTraffic_Should_Fail_When_DensityLevel_Is_Invalid()
    {
        var fakeRepo = new FakeTrafficRepository();
        var service = new TrafficService(fakeRepo);

        var request = new TrafficRecordRequest
        {
            LocationId = "Kadikoy",
            VehicleCount = 100,
            DensityLevel = "Extreme"
        };

        var result = await service.RecordTrafficAsync(request);

        Assert.False(result.Success);
        Assert.Equal("Geçersiz yoğunluk seviyesi. (Low, Medium, High olmalıdır)", result.ErrorMessage);
    }

    [Fact]
    public async Task GetHotspots_Should_Return_Only_High_Density_Records()
    {
        var fakeRepo = new FakeTrafficRepository();

        await fakeRepo.CreateAsync(new TrafficRecord
        {
            LocationId = "Kadikoy",
            VehicleCount = 100,
            DensityLevel = "High"
        });

        await fakeRepo.CreateAsync(new TrafficRecord
        {
            LocationId = "Besiktas",
            VehicleCount = 60,
            DensityLevel = "Medium"
        });

        var service = new TrafficService(fakeRepo);

        var result = await service.GetHotspotsAsync();

        Assert.Single(result);
        Assert.Equal("Kadikoy", result.First().LocationId);
        Assert.Equal("High", result.First().DensityLevel);
    }

}