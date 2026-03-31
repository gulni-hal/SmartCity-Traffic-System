using System.Threading.Tasks;
using Traffic.Application.Entities;

namespace Traffic.Application.Interfaces;

public interface ITrafficRepository
{
    Task CreateAsync(TrafficRecord record);
}