using MySmartCityWebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImageStorageDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task<IEnumerable<Photo>> UploadAsync(StorageFile file)
        {
            using (var client = new HttpClient())
            {
                using (var content =
                    new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    using (var fileStream = await file.OpenStreamForReadAsync())
                    {
                        //on pourrait uploader plusieurs images en même temps. Par simplicité je n'en uploade qu'une.
                        content.Add(new StreamContent(fileStream), "NouveauFichier", file.Name);

                        //using (
                        //   var message =
                        HttpResponseMessage result = await client.PostAsync("http://localhost:57814/api/Photo", content);
                        if (!result.IsSuccessStatusCode)
                        {
                            var dialog = new MessageDialog("Problème lors de l'upload");
                            await dialog.ShowAsync();
                            return new Photo[0];
                        }
                        else
                        {
                            var jsonResponse = await result.Content.ReadAsStringAsync();
                            IEnumerable<Photo> savedPhotos = JsonConvert.DeserializeObject<Photo[]>(jsonResponse);
                            return savedPhotos;
                        }

                        //{
                        //    var input = await message.Content.ReadAsStringAsync();

                        //    return !string.IsNullOrWhiteSpace(input) ? Regex.Match(input, @"http://\w*\.directupload\.net/images/\d*/\w*\.[a-z]{3}").Value : null;
                        //}
                    }

                }
            }
        }

        private void DisplayPictures(IEnumerable<Photo> savedPhotos)
        {
            LastImageUploaded.Source = null;
            //on peut en recevoir plusieurs => par défaut je n'en affiche qu'une.
            if (!savedPhotos.Any())
                return;
            LastImageUploaded.Source = new BitmapImage(
             new Uri(savedPhotos.First().Uri, UriKind.Absolute));
            LastImageUploadedUrl.Text = savedPhotos.First().Uri;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await PickFileAsync();
            IEnumerable<Photo> uploadedFiles=await UploadAsync(file);
            DisplayPictures(uploadedFiles);
        }

        private async Task<StorageFile> PickFileAsync()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            StorageFile file = await picker.PickSingleFileAsync();
            return file;
        }
    }
}
