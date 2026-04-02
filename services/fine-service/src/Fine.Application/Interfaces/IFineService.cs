using System.Collections.Generic;
using System.Threading.Tasks;
using Fine.Application.DTOs;

namespace Fine.Application.Interfaces;

public interface IFineService
{
    Task<FineResult> CreateFineAsync(CreateFineRequest request);
    // YENİ EKLENEN
    Task<IEnumerable<FineRecordResponse>> GetFinesByPlateAsync(string licensePlate);
    Task<IEnumerable<FineRecordResponse>> GetAllFinesAsync();
    Task<bool> DeleteFineAsync(string id);
}