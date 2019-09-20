using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace MyUpload
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        MediaFile ImageFile;

        public MainPage()
        {
            InitializeComponent();
        }

        async void TakePhoto_Clicked(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":) No camera avaiable", "Close");
                return;
            }

            if (ImageFile != null)
            {
                ImageFile.Dispose();
            }

            ImageFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            { });

            if (ImageFile == null)
            {
                return;
            }

            Console.WriteLine(ImageFile.AlbumPath);

            MainImage.Source = ImageSource.FromStream(() =>
            {
                var stream = ImageFile.GetStream();
                return stream;
            });

        }

        async void PickPhoto_Clicked(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No Photo", ":( No Photo avaiable", "Close");
                return;
            }

            if (ImageFile != null)
            {
                ImageFile.Dispose();
            }

            ImageFile = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions { });

            if (ImageFile == null)
            {
                return;
            }

            Console.WriteLine(ImageFile.AlbumPath);

            MainImage.Source = ImageSource.FromStream(() =>
            {
                var stream = ImageFile.GetStream();
                return stream;
            });

        }


        void UploadPhoto_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                byte[] imageByte = ReadFully(ImageFile.GetStream());
                const string imageKey = "userfile";  
                const string imageName = "dummy.jpeg";

                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imageByte), imageKey, imageName);

                // Add content (optional)
                content.Add(new StringContent("I LOVE CAT"), "username");
                content.Add(new StringContent("xamarin core"), "password");

                const string url = "http://172.17.9.49:1112/uploads";
                var result = new HttpClient().PostAsync(new Uri(url), content).Result;

                DisplayAlert("Response", result.Content.ReadAsStringAsync().Result, "CLOSE");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Convert stream to byte array function
        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


    }
}
