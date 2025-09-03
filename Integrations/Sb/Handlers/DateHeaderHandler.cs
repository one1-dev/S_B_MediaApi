using System.Net.Http;

namespace S_B_MicroService.Integrations.Sb;

public sealed class DateHeaderHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
    {
        req.Headers.Date = DateTimeOffset.UtcNow;   // S&B expects a valid Date header (±15m)
        return base.SendAsync(req, ct);
    }
}
