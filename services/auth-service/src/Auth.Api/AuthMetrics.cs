using Prometheus;

namespace Auth.Api;

public static class AuthMetrics
{
    public static readonly Counter RegisterSuccess = Metrics
        .CreateCounter("auth_register_success_total", "Başarılı kayıt sayısı");

    public static readonly Counter LoginSuccess = Metrics
        .CreateCounter("auth_login_success_total", "Başarılı login sayısı");

    public static readonly Counter LoginFailed = Metrics
        .CreateCounter("auth_login_failed_total", "Başarısız login sayısı");

    public static readonly Counter LogoutSuccess = Metrics
        .CreateCounter("auth_logout_success_total", "Başarılı logout sayısı");
}
