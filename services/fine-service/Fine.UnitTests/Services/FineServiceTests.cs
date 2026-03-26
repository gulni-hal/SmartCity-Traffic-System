using Xunit;
using System.Threading.Tasks;
using Fine.Application.Services;

namespace Fine.UnitTests.Services;

public class FineServiceTests
{
    [Fact]
    public async Task CreateFine_Should_Return_Success_When_Valid_Data()
    {
        //arrange
        var service = new FineService();
        var request = new CreateFineRequest
        {
            LicensePlate = "34ABC123",
            Amount = 1500.50m,
            Reason = "Kırmızı Işık İhlali"
        };

        //act
        var result = await service.CreateFineAsync(request);

        //assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("34ABC123", result.Data.LicensePlate);
    }
}