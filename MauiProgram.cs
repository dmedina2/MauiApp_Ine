using MauiApp1.DataAccess;
using MauiApp1.Platforms.Android;
using MauiApp1.Services;
using Microsoft.Extensions.Logging;
using TesseractOcrMaui;

namespace MauiApp1
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddTesseractOcr(
            files =>
            {
                // must have matching files in Resources/Raw folder
                files.AddFile("spa.traineddata");
            });
            builder.Services.AddSingleton<Views.MainPage>();
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddDbContext<IneDbContext>();
            builder.Services.AddSingleton(Connectivity.Current);
            return builder.Build();
        }
    }
}
