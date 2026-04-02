using System.Collections.Generic;
using System.Threading.Tasks;
using Traffic.Application.DTOs;

namespace Traffic.Application.Interfaces;

public interface ITrafficService
{
    Task<TrafficRecordResult> RecordTrafficAsync(TrafficRecordRequest request);
    Task<IEnumerable<TrafficRecordResponse>> GetTrafficByLocationAsync(string locationId);
    Task<IEnumerable<TrafficRecordResponse>> GetHotspotsAsync(); // Hocanın istediği üst seviye özellik
}