using CommunityToolkit.Mvvm.Input;
using MauiApp1.DataAccess;
using MauiApp1.Model;
using MauiApp1.Services;
using Microsoft.Maui.Controls;
using System.Net.Security;
using TesseractOcrMaui;
using TesseractOcrMaui.Results; // Include library namespace

namespace MauiApp1.Views;
public partial class MainPage : ContentPage
{
    private readonly IConnectivity _connectivity;
    UploadFile uploadFile { get; set; }
    private readonly IneDbContext _db;
    FileResult? photo = null;
    // inject ITesseract
    public MainPage(ITesseract tesseract, IneDbContext ineDbContext, IConnectivity connectivity)
    {
        InitializeComponent();
        Tesseract = tesseract;
        _db = ineDbContext;
        uploadFile = new UploadFile();
        _connectivity = connectivity;
        _connectivity.ConnectivityChanged += _connectivity_ConnectivityChanged;
    }

    private void _connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public bool StatusConnection()
    {
        return _connectivity.NetworkAccess == NetworkAccess.Internet ? true : false;
    }

    ITesseract Tesseract { get; }

    private async void UploadImagen_Clicked(object sender, EventArgs e)
    {
        if (entorno.IsToggled)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using Stream sourceStream = await photo.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(localFilePath);

                    await sourceStream.CopyToAsync(localFileStream);
                    var result = await Tesseract.RecognizeTextAsync(localFilePath);

                    // Show output
                    confidenceLabel.Text = $"Resultado: {result.Confidence}";
                    if (result.NotSuccess())
                    {
                        resultLabel.Text = $"Error en la lectura del INE: {result.Status}";
                        return;
                    }
                    resultLabel.Text = "Lectura de INE";
                    string[] splt = result.RecognisedText.Split(' ');
                    var cont = splt.Where(c => c.Count() == 4).ToList();
                    seccion.Text = cont.Last().ToString();
                }
            }
        }
        else
        {
            //Make user pick file
            photo = await FilePicker.PickAsync(new PickOptions()
            {
                PickerTitle = "Pick jpeg or png image",
                // Currently usable image types
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
                {
                    [DevicePlatform.Android] = new List<string>() { "image/png", "image/jpeg" },
                    [DevicePlatform.WinUI] = new List<string>() { ".png", ".jpg", ".jpeg" },
                })
            });
            // null if user cancelled the operation
            if (photo is null)
            {
                return;
            }
            //Recognize image
            var result = await Tesseract.RecognizeTextAsync(photo.FullPath);

            // Show output
            confidenceLabel.Text = $"Resultado: {result.Confidence}";
            if (result.NotSuccess())
            {
                resultLabel.Text = $"Error en la lectura del INE: {result.Status}";
                return;
            }
            resultLabel.Text = "Lectura de INE";
            string[] splt = result.RecognisedText.Split(' ');
            var cont = splt.Where(c => c.Count() == 4).ToList();
            seccion.Text = cont.Last().ToString();
        }

    }

    private async void Guardar_Info(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(seccion.Text))
        {
            ImagenFile imagenFile = null;
            if (photo != null)
            {
                imagenFile = await uploadFile.Upload(photo);
            }
            _db.Database.EnsureCreated();
            Ine ine = new Ine(imagenFile.ByteBase64, imagenFile.ContentType, imagenFile.FileName, seccion.Text, "");
            _db.Ines.Add(ine);
            await _db.SaveChangesAsync();
            await Shell.Current.DisplayAlert("Mensaje", "Información almacenada", "Aceptar");
            DataSql.Text = "Información actual en DB: " + _db.Ines.Count();
        }
        else
        {
            await Shell.Current.DisplayAlert("Mensaje", "La sección es obligatoria", "Aceptar");
        }
    }

    [RelayCommand(CanExecute =nameof(StatusConnection))]
    private void EnviarInformacion()
    {

    }
}


