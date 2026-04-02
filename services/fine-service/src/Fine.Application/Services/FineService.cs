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
        if (string.IsNullOrWhiteSpace(request.LicensePlate) || request.LicensePlate.Length < 5)
        {
            return new FineResult { Success = false, ErrorMessage = "Geçersiz plaka formatı." };
        }

        if (request.Amount <= 0)
        {
            return new FineResult { Success = false, ErrorMessage = "Ceza tutarı sıfırdan büyük olmalıdır." };
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return new FineResult { Success = false, ErrorMessage = "Ceza nedeni boş bırakılamaz." };
        }
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
    public async Task<IEnumerable<FineRecordResponse>> GetAllFinesAsync()
    {
        var records = await _fineRepository.GetAllAsync();
        return records.Select(r => new FineRecordResponse { LicensePlate = r.LicensePlate, Amount = r.Amount, Reason = r.Reason, CreatedAt = r.CreatedAt });
    }

    public async Task<bool> DeleteFineAsync(string id)
    {
        await _fineRepository.DeleteAsync(id);
        return true;
    }
}