namespace Reto8
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            ClasPicker.ItemsSource = new List<string> { "Todas", Clasif.Consultoria, Clasif.Medida, Clasif.Fabrica };
            ClasPicker.SelectedIndex = 0;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Cargar();
        }

        void Cargar()
        {
            Lista.ItemsSource = Database.Listar(BuscarBar.Text ?? "", ClasPicker.SelectedItem as string ?? "Todas");
        }

        void OnFiltroChanged(object? sender, EventArgs e) => Cargar();

        async void OnNueva(object? sender, EventArgs e) =>
            await Navigation.PushAsync(new EmpresaPage(new Empresa()));

        async void OnSeleccion(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Empresa emp)
            {
                Lista.SelectedItem = null;
                await Navigation.PushAsync(new EmpresaPage(emp));
            }
        }
    }
}
