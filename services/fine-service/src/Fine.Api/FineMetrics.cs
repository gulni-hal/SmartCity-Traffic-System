using Prometheus;

namespace Fine.Api;

public static class FineMetrics
{
    public static readonly Counter FineCreated = Metrics
        .CreateCounter("fine_created_total", "Oluşturulan ceza sayısı");

    public static readonly Counter FineDeleted = Metrics
        .CreateCounter("fine_deleted_total", "Silinen ceza sayısı");

    public static readonly Counter FineValidationFailed = Metrics
        .CreateCounter("fine_validation_failed_total", "Validation hatalı ceza istekleri");
}
