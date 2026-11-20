using reto10.Models;

namespace reto10.Services;

public interface IPuntosService
{
    string UltimaUrl { get; }
    Task<IReadOnlyList<PuntoPosconsumo>> BuscarAsync(string? texto, string? municipio, int limite, CancellationToken ct = default);
}
