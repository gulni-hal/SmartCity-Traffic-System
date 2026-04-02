namespace Fine.Application.DTOs;

public class FineResult
{
    public bool Success { get; set; }
    public FineData? Data { get; set; }
    public string? ErrorMessage { get; set; } // Hata mesajı için eklendi
}