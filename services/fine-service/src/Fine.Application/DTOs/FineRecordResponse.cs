using System;

namespace Fine.Application.DTOs;

public class FineRecordResponse
{
    public string Id { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
