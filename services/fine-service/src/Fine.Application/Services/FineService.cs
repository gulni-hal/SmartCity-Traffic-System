using System.Threading.Tasks;
using Fine.Application.DTOs;
using Fine.Application.Interfaces;

namespace Fine.Application.Services;

public class FineService : IFineService
{
    public async Task<FineResult> CreateFineAsync(CreateFineRequest request)
    {
        var result = new FineResult
        {
            Success = true,
            Data = new FineData
            {
                LicensePlate = request.LicensePlate
            }
        };

        return await Task.FromResult(result);
    }
}