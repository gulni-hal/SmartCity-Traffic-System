using System;

namespace Traffic.Application.Entities;

public class TrafficRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string LocationId { get; set; } = string.Empty;
    public int VehicleCount { get; set; }
    public string DensityLevel { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}