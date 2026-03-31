using Xunit;
using System.Threading.Tasks;
using Traffic.Application.Services;

namespace Traffic.UnitTests.Services;

public class TrafficServiceTests
{
    [Fact]
    public async Task RecordTraffic_Should_Return_Success_When_Valid_Data()
    {
        // Arrange (Hazırlık)
        var service = new TrafficService();
        var request = new TrafficRecordRequest
        {
            LocationId = "Kadikoy-Merkez-1",
            VehicleCount = 120,
            DensityLevel = "High"
        };

        // Act (Eylem)
        var result = await service.RecordTrafficAsync(request);

        // Assert (Doğrulama)
        Assert.True(result.Success);
        Assert.Equal("Trafik verisi basariyla kaydedildi.", result.Message);
    }
}