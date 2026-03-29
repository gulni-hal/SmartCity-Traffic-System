using System;

namespace Fine.Application.Entities;

public class FineRecord
{
    // MongoDB ID'si
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string LicensePlate { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}