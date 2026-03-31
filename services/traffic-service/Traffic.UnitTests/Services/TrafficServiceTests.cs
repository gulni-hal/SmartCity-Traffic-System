using Xunit;
using System.Threading.Tasks;
using Traffic.Application.Services;
using Traffic.Application.Interfaces;
using Traffic.Application.Entities;

namespace Traffic.UnitTests.Services;

public class FakeTrafficRepository : ITrafficRepository
{
    public Task CreateAsync(TrafficRecord record) => Task.CompletedTask;
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