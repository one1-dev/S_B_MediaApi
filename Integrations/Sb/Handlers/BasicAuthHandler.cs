using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace S_B_MicroService.Integrations.Sb;

public sealed class BasicAuthHandler : DelegatingHandler
{
    private readonly AuthenticationHeaderValue _auth;
    public BasicAuthHandler(string user, string pass)
    {
        var raw = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}"));
        _auth = new AuthenticationHeaderValue("Basic", raw);
    }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
    {
        req.Headers.Authorization = _auth;
        return base.SendAsync(req, ct);
    }
}
