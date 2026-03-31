using System;
using System.Threading.Tasks;

namespace Traffic.Application.Services;

// Trafik yoğunluğu kaydı için dışarıdan alacağımız veriler
public class TrafficRecordRequest
{
    public string LocationId { get; set; } = string.Empty;
    public int VehicleCount { get; set; }
    public string DensityLevel { get; set; } = string.Empty; // Low, Medium, High
}

// İşlem sonucu döneceğimiz veriler
public class TrafficRecordResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class TrafficService
{
    public Task<TrafficRecordResult> RecordTrafficAsync(TrafficRecordRequest request)
    {
        // TDD Red Aşaması: Henüz kodu yazmadık, bilerek patlatıyoruz!
        throw new NotImplementedException();
    }
}