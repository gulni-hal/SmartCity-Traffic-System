using System.Collections.Generic;
using System.Threading.Tasks;
using Fine.Application.Entities;

namespace Fine.Application.Interfaces;

public interface IFineRepository
{
    Task CreateAsync(FineRecord record);
    // YENİ EKLENEN
    Task<IEnumerable<FineRecord>> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<FineRecord>> GetAllAsync();
    Task DeleteAsync(string id);
}