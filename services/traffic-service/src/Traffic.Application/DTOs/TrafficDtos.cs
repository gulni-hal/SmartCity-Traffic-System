using System;

namespace Traffic.Application.DTOs;

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
    public string? ErrorMessage { get; set; } // İş kuralları hataları için eklendi
}

public class TrafficRecordResponse
{
    public string LocationId { get; set; } = string.Empty;
    public int VehicleCount { get; set; }
    public string DensityLevel { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
}