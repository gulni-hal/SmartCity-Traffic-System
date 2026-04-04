using Prometheus;

namespace Traffic.Api;

public static class TrafficMetrics
{
    public static readonly Counter TrafficRecorded = Metrics
        .CreateCounter("traffic_recorded_total", "Kaydedilen trafik verisi sayısı");

    public static readonly Counter TrafficValidationFailed = Metrics
        .CreateCounter("traffic_validation_failed_total", "Validation hatalı trafik istekleri");

    public static readonly Counter HotspotsRequested = Metrics
        .CreateCounter("traffic_hotspots_requested_total", "Hotspots sorgu sayısı");
}
