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
    public async Task<TrafficRecordResult> RecordTrafficAsync(TrafficRecordRequest request)
    {
        // TDD Green Aşaması: Şimdilik testin geçmesi için gereken en basit ve doğru kodu yazıyoruz.
        // İleride buraya MongoDB'ye kaydetme mantığını (Repository) ekleyeceğiz.

        var result = new TrafficRecordResult
        {
            Success = true,
            Message = "Trafik verisi basariyla kaydedildi."
        };

        return await Task.FromResult(result);
    }
}