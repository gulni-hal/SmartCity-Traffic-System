using System.Threading.Tasks;
using Fine.Application.Entities;

namespace Fine.Application.Interfaces;

public interface IFineRepository
{
    Task CreateAsync(FineRecord record);
}