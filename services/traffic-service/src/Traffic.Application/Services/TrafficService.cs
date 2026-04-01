using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffic.Application.Interfaces;
using Traffic.Application.Entities;

namespace Traffic.Application.Services;

public class TrafficRecordRequest
{
    public string LocationId { get; set; } = string.Empty;
    public int VehicleCount { get; set; }
    public string DensityLevel { get; set; } = string.Empty;
}

public class TrafficRecordResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

// YENİ EKLENEN: Dışarıya döneceğimiz veri modeli
public class TrafficRecordResponse
{
    public string LocationId { get; set; } = string.Empty;
    public int VehicleCount { get; set; }
    public string DensityLevel { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
}

public class TrafficService
{
    private readonly ITrafficRepository _repository;

    public TrafficService(ITrafficRepository repository)
    {
        _repository = repository;
    }

    public async Task<TrafficRecordResult> RecordTrafficAsync(TrafficRecordRequest request)
    {
        var record = new TrafficRecord
        {
            LocationId = request.LocationId,
            VehicleCount = request.VehicleCount,
            DensityLevel = request.DensityLevel
        };

        await _repository.CreateAsync(record);

        return new TrafficRecordResult
        {
            Success = true,
            Message = "Trafik verisi basariyla kaydedildi."
        };
    }

    // YENİ EKLENEN METOT: Verileri getir ve DTO'ya dönüştür
    public async Task<IEnumerable<TrafficRecordResponse>> GetTrafficByLocationAsync(string locationId)
    {
        var records = await _repository.GetByLocationIdAsync(locationId);

        return records.Select(r => new TrafficRecordResponse
        {
            LocationId = r.LocationId,
            VehicleCount = r.VehicleCount,
            DensityLevel = r.DensityLevel,
            RecordedAt = r.RecordedAt
        });
    }
}