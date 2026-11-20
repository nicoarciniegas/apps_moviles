using reto10.ViewModels;

namespace reto10.Views;

public partial class BuscarPage : ContentPage
{
    public BuscarPage(BuscarViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
