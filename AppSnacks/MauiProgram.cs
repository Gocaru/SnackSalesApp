using AppSnacks.Services;
using AppSnacks.Validations;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace AppSnacks
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<FavouritesService>();
            builder.Services.AddSingleton<IValidator, Validator>();

            return builder.Build();
        }
    }
}
