using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffic.Application.Interfaces;
using Traffic.Application.Entities;
using Traffic.Application.DTOs;

namespace Traffic.Application.Services;

public class TrafficService : ITrafficService
{
    private readonly ITrafficRepository _repository;

    public TrafficService(ITrafficRepository repository)
    {
        _repository = repository;
    }

    public async Task<TrafficRecordResult> RecordTrafficAsync(TrafficRecordRequest request)
    {
        // 1. İŞ KURALI: Lokasyon ID boş olamaz
        if (string.IsNullOrWhiteSpace(request.LocationId))
        {
            return new TrafficRecordResult { Success = false, ErrorMessage = "Lokasyon bilgisi boş olamaz." };
        }

        // 2. İŞ KURALI: Araç sayısı negatif olamaz
        if (request.VehicleCount < 0)
        {
            return new TrafficRecordResult { Success = false, ErrorMessage = "Araç sayısı negatif olamaz." };
        }

        // 3. İŞ KURALI: Sadece belirli yoğunluk seviyeleri kabul edilebilir
        var validDensities = new[] { "Low", "Medium", "High" };
        if (!validDensities.Contains(request.DensityLevel))
        {
            return new TrafficRecordResult { Success = false, ErrorMessage = "Geçersiz yoğunluk seviyesi. (Low, Medium, High olmalıdır)" };
        }

        var record = new TrafficRecord
        {
            LocationId = request.LocationId,
            VehicleCount = request.VehicleCount,
            DensityLevel = request.DensityLevel
        };

        await _repository.CreateAsync(record);

        return new TrafficRecordResult { Success = true, Message = "Trafik verisi basariyla kaydedildi." };
    }

    public async Task<IEnumerable<TrafficRecordResponse>> GetTrafficByLocationAsync(string locationId)
    {
        var records = await _repository.GetByLocationIdAsync(locationId);
        return records.Select(r => new TrafficRecordResponse { LocationId = r.LocationId, VehicleCount = r.VehicleCount, DensityLevel = r.DensityLevel, RecordedAt = r.RecordedAt });
    }

    // ÜST DÜZEY İHTİYAÇ: Hotspot (Kritik Yoğunluk) Analizi
    public async Task<IEnumerable<TrafficRecordResponse>> GetHotspotsAsync()
    {
        var records = await _repository.GetHotspotsAsync();
        return records.Select(r => new TrafficRecordResponse { LocationId = r.LocationId, VehicleCount = r.VehicleCount, DensityLevel = r.DensityLevel, RecordedAt = r.RecordedAt });
    }
}