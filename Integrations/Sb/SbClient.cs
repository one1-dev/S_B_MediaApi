using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using S_B_MicroService.Integrations.Sb.Models;

namespace S_B_MicroService.Integrations.Sb;

public sealed class SbClient(HttpClient http) : ISbClient
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<ContractDetailResponse> GetContractDetailAsync(int id, CancellationToken ct)
        => await http.GetFromJsonAsync<ContractDetailResponse>($"contracts/{id}/detail", Json, ct)
           ?? throw new InvalidOperationException("Empty S&B response");

    public async Task UpdateContractDetailAsync(int id, ContractDetailUpdate body, CancellationToken ct)
    {
        using var resp = await http.PutAsJsonAsync($"contracts/{id}/detail", body, Json, ct);
        await EnsureSuccess(resp, ct);
    }

    public async Task<ConsumersDetailResponse> GetConsumersDetailAsync(int id, string? lpn, CancellationToken ct)
    {
        var url = $"contracts/{id}/consumersdetail" + (string.IsNullOrWhiteSpace(lpn) ? "" : $"?lpn={Uri.EscapeDataString(lpn)}");
        using var resp = await http.GetAsync(url, ct);
        await EnsureSuccess(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<ConsumersDetailResponse>(Json, ct))!;
    }

    static async Task EnsureSuccess(HttpResponseMessage resp, CancellationToken ct)
    {
        if (resp.IsSuccessStatusCode) return;
        var body = await resp.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException($"S&B error {(int)resp.StatusCode}: {body}");
    }
}
