using MauiApp1.Services;

namespace MauiApp1.Platforms.Android;
public class DatabaseService : IDatabaseService
{
    public string Get(string nombreArchivo)
    {
        var ruta = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        return Path.Combine(ruta, nombreArchivo);
    }
}

