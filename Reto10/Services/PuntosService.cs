using System.Text.Json;
using reto10.Models;

namespace reto10.Services;

public class PuntosService : IPuntosService
{
    public const string BaseUrl = "https://www.datos.gov.co/resource/c2fr-ezse.json";
    private readonly HttpClient _http;

    public string UltimaUrl { get; private set; } = string.Empty;

    public PuntosService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<PuntoPosconsumo>> BuscarAsync(string? texto, string? municipio, int limite, CancellationToken ct = default)
    {
        var query = new List<string> { $"$limit={limite}" };

        if (!string.IsNullOrWhiteSpace(texto))
            query.Add($"$q={Uri.EscapeDataString(texto.Trim())}");

        if (!string.IsNullOrWhiteSpace(municipio))
        {
            var valor = municipio.Trim().ToUpperInvariant().Replace("'", "''");
            var where = $"upper({Campos.Municipio}) like '%{valor}%'";
            query.Add($"$where={Uri.EscapeDataString(where)}");
        }

        UltimaUrl = $"{BaseUrl}?{string.Join("&", query)}";

        using var response = await _http.GetAsync(UltimaUrl, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

        var lista = new List<PuntoPosconsumo>();
        if (doc.RootElement.ValueKind == JsonValueKind.Array)
            foreach (var el in doc.RootElement.EnumerateArray())
                lista.Add(PuntoPosconsumo.From(el));
        return lista;
    }
}
