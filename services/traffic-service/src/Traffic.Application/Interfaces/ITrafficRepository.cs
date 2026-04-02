using System.Collections.Generic;
using System.Threading.Tasks;
using Traffic.Application.Entities;

namespace Traffic.Application.Interfaces;

public interface ITrafficRepository
{
    Task CreateAsync(TrafficRecord record);
    // YENİ EKLENEN: Lokasyona göre trafik verilerini getirme
    Task<IEnumerable<TrafficRecord>> GetByLocationIdAsync(string locationId);
    Task<IEnumerable<TrafficRecord>> GetHotspotsAsync();
}