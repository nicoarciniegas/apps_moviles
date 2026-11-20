using Microsoft.Extensions.Logging;

namespace reto10
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(45) });
            builder.Services.AddSingleton<Services.IPuntosService, Services.PuntosService>();
            builder.Services.AddTransient<ViewModels.BuscarViewModel>();
            builder.Services.AddTransient<Views.BuscarPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
