namespace Reto8
{
    public static class Clasif
    {
        public const string Consultoria = "Consultor\u00eda";
        public const string Medida = "Desarrollo a la medida";
        public const string Fabrica = "F\u00e1brica de software";
    }

    public partial class EmpresaPage : ContentPage
    {
        readonly Empresa _emp;

        public EmpresaPage(Empresa emp)
        {
            InitializeComponent();
            _emp = emp;
            NombreEntry.Text = emp.Nombre;
            UrlEntry.Text = emp.Url;
            TelEntry.Text = emp.Telefono;
            EmailEntry.Text = emp.Email;
            ProdEditor.Text = emp.Productos;
            var cl = emp.Clasificacion.Split(',');
            ConsultoriaCheck.IsChecked = cl.Contains(Clasif.Consultoria);
            MedidaCheck.IsChecked = cl.Contains(Clasif.Medida);
            FabricaCheck.IsChecked = cl.Contains(Clasif.Fabrica);
            EliminarBtn.IsVisible = emp.Id != 0;
        }

        async void OnGuardar(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreEntry.Text))
            {
                await DisplayAlert("Error", "El nombre es obligatorio", "OK");
                return;
            }
            var cl = new List<string>();
            if (ConsultoriaCheck.IsChecked) cl.Add(Clasif.Consultoria);
            if (MedidaCheck.IsChecked) cl.Add(Clasif.Medida);
            if (FabricaCheck.IsChecked) cl.Add(Clasif.Fabrica);

            _emp.Nombre = NombreEntry.Text.Trim();
            _emp.Url = UrlEntry.Text ?? "";
            _emp.Telefono = TelEntry.Text ?? "";
            _emp.Email = EmailEntry.Text ?? "";
            _emp.Productos = ProdEditor.Text ?? "";
            _emp.Clasificacion = string.Join(",", cl);
            Database.Guardar(_emp);
            await Navigation.PopAsync();
        }

        async void OnEliminar(object? sender, EventArgs e)
        {
            if (await DisplayAlert("Confirmar", $"\u00bfEliminar {_emp.Nombre}?", "S\u00ed", "No"))
            {
                Database.Eliminar(_emp);
                await Navigation.PopAsync();
            }
        }
    }
}
