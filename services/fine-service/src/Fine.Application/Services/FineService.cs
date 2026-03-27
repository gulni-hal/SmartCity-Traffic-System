using System;
using System.Threading.Tasks;

namespace Fine.Application.Services;

//trafik cezasi olusturulurken disaridan alinacak veriler icin
public class CreateFineRequest
{
    public string LicensePlate { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

// ceza olusturulduktan sonra disarirya dondurecegimiz veriler
public class FineData
{
    public string LicensePlate { get; set; } = string.Empty;
}

public class FineResult
{
    public bool Success { get; set; }
    public FineData? Data { get; set; }
}

public class FineService
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