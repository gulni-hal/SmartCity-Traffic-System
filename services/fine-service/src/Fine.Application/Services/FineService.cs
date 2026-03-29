using System.Threading.Tasks;
using Fine.Application.DTOs;
using Fine.Application.Interfaces;
using Fine.Application.Entities;

namespace Fine.Application.Services;

public class FineService : IFineService
{
    private readonly IFineRepository _fineRepository;

    // Veritabanı bağımlılığımızı (DI) ekliyoruz
    public FineService(IFineRepository fineRepository)
    {
        _fineRepository = fineRepository;
    }

    public async Task<FineResult> CreateFineAsync(CreateFineRequest request)
    {
        // 1. Gelen isteği veritabanı modeline çevir
        var newFine = new FineRecord
        {
            LicensePlate = request.LicensePlate,
            Amount = request.Amount,
            Reason = request.Reason
        };

        // 2. Veritabanına kaydet
        await _fineRepository.CreateAsync(newFine);

        // 3. Başarılı sonucu dön
        var result = new FineResult
        {
            Success = true,
            Data = new FineData { LicensePlate = newFine.LicensePlate }
        };

        return await Task.FromResult(result);
    }
}