using System.Collections.Generic;
using System.Threading.Tasks;
using Fine.Application.Entities;

namespace Fine.Application.Interfaces;

public interface IFineRepository
{
    Task CreateAsync(FineRecord record);
    Task<IEnumerable<FineRecord>> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<FineRecord>> GetAllAsync();
    Task<bool> DeleteAsync(string id);
}
