using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using reto10.Models;
using reto10.Services;

namespace reto10.ViewModels;

public class BuscarViewModel : INotifyPropertyChanged
{
    private readonly IPuntosService _service;
    private string? _texto, _municipio;
    private int _limite = 25;
    private bool _isBusy, _hayError;
    private string _mensaje = "Ingrese parámetros y pulse Buscar.";
    private string _url = string.Empty;

    public BuscarViewModel(IPuntosService service)
    {
        _service = service;
        BuscarCommand = new Command(async () => await BuscarAsync(), () => !IsBusy);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Texto { get => _texto; set => Set(ref _texto, value); }
    public string? Municipio { get => _municipio; set => Set(ref _municipio, value); }
    public int Limite { get => _limite; set => Set(ref _limite, value); }
    public IList<int> Limites { get; } = new List<int> { 10, 25, 50 };
    public ObservableCollection<PuntoPosconsumo> Resultados { get; } = new();
    public string Mensaje { get => _mensaje; set => Set(ref _mensaje, value); }
    public string Url { get => _url; set => Set(ref _url, value); }
    public bool HayError { get => _hayError; set => Set(ref _hayError, value); }
    public bool IsBusy
    {
        get => _isBusy;
        set { if (Set(ref _isBusy, value)) ((Command)BuscarCommand).ChangeCanExecute(); }
    }
    public ICommand BuscarCommand { get; }

    private async Task BuscarAsync()
    {
        IsBusy = true;
        HayError = false;
        Mensaje = "Buscando...";
        Resultados.Clear();
        try
        {
            var lista = await _service.BuscarAsync(Texto, Municipio, Limite);
            Url = _service.UltimaUrl;
            foreach (var p in lista) Resultados.Add(p);
            Mensaje = lista.Count == 0 ? "No se encontraron resultados." : $"{lista.Count} resultados";
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            HayError = true;
            System.Diagnostics.Debug.WriteLine($"Error consulta: {ex}");
            Mensaje = $"No se pudo consultar el servicio. Revise su conexión e intente de nuevo. ({ex.GetType().Name}: {ex.Message})";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        return true;
    }
}
