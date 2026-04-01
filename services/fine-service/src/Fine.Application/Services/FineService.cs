using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fine.Application.DTOs;
using Fine.Application.Interfaces;
using Fine.Application.Entities;

namespace Fine.Application.Services;

public class FineService : IFineService
{
    private readonly IFineRepository _fineRepository;

    public FineService(IFineRepository fineRepository)
    {
        _fineRepository = fineRepository;
    }

    public async Task<FineResult> CreateFineAsync(CreateFineRequest request)
    {
        var newFine = new FineRecord
        {
            LicensePlate = request.LicensePlate,
            Amount = request.Amount,
            Reason = request.Reason
        };

        await _fineRepository.CreateAsync(newFine);

        return new FineResult
        {
            Success = true,
            Data = new FineData { LicensePlate = newFine.LicensePlate }
        };
    }

    // YENİ EKLENEN METOT
    public async Task<IEnumerable<FineRecordResponse>> GetFinesByPlateAsync(string licensePlate)
    {
        var records = await _fineRepository.GetByLicensePlateAsync(licensePlate);

        return records.Select(r => new FineRecordResponse
        {
            LicensePlate = r.LicensePlate,
            Amount = r.Amount,
            Reason = r.Reason,
            CreatedAt = r.CreatedAt
        });
    }
}