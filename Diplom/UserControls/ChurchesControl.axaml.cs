using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Avalonia.Interactivity;

namespace Diplom.UserControls
{
    public partial class ChurcesControl : UserControl
    {
        private int Idregion;
        public ChurcesControl(int regionId)
        {
            InitializeComponent();
            Idregion = regionId;
            LoadChurches(regionId);
        }

     private async void LoadChurches(int regionId)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5249/");
                HttpResponseMessage response = await client.GetAsync($"api/churches/region/{regionId}");
                response.EnsureSuccessStatusCode();

                var churches = await response.Content.ReadFromJsonAsync<List<AllDTO.ChurchDto>>();
                if (churches != null)
                {
                    foreach (var church in churches)
                    {
                        HttpResponseMessage photoResponse = await client.GetAsync($"api/churches/{church.Id}/photos");
                        if (photoResponse.IsSuccessStatusCode)
                        {
                            var photos = await photoResponse.Content.ReadFromJsonAsync<List<AllDTO.PhotoDto>>();
                            if (photos != null && photos.Count > 0)
                            {
                                church.Photos = new List<string>();
                                foreach (var photo in photos)
                                {
                                    var imageUrl = $"http://localhost:5249/api/churches/photos/bytes/{photo.NamePhoto}";
                                    church.Photos.Add(imageUrl);
                                }

                                if (church.Photos.Count > 0)
                                {
                                    var imageUrl = church.Photos[0];
                                    try
                                    {
                                        using (var stream = await client.GetStreamAsync(imageUrl))
                                        {
                                            var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
                                            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                                            {
                                                await stream.CopyToAsync(fileStream);
                                            }

                                            church.FirstPhoto = new Bitmap(tempFilePath);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Unable to load bitmap for church {church.Id}: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }

                    var churchesListBox = this.FindControl<ListBox>("ChurchesListBox");
                    if (churchesListBox != null)
                    {
                        churchesListBox.ItemsSource = churches;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message}");
        }
    }

        private void ChurchesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var churchesListBox = this.FindControl<ListBox>("ChurchesListBox");
            if (churchesListBox == null)
            {
                Console.WriteLine("ChurchesListBox is null.");
                return;
            }

            var selectedChurch = (AllDTO.ChurchDto)churchesListBox.SelectedItem;
            if (selectedChurch != null)
            {
                ChurchDetailControl churchDetailControl = new ChurchDetailControl(selectedChurch, Idregion);
                NavigationManager.NavigateTo(churchDetailControl);
                
            }
            
        }
        

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnBackButtonClick(object? sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new MapControl());
        }
    }
}