using System.Threading.Tasks;
using Fine.Application.DTOs;

namespace Fine.Application.Interfaces;

public interface IFineService
{
    Task<FineResult> CreateFineAsync(CreateFineRequest request);
}