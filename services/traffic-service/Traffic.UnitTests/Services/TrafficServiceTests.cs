using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffic.Application.Services;
using Traffic.Application.Interfaces;
using Traffic.Application.Entities;

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
}