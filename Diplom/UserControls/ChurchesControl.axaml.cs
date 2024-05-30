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
    public partial class ChurchesControl : UserControl
    {
        private int Idregion;
        private bool IsRussian;

        public ChurchesControl(int regionId, bool isRussian)
        {
            InitializeComponent();
            Idregion = regionId;
            IsRussian = isRussian;
            LoadChurches(regionId);
            UpdateUIForLanguage();
        }

        private async void LoadChurches(int regionId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://194.146.242.26:6666/");
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
                                    var firstPhoto = photos[0];
                                    var imageUrl = $"http://localhost:5249/api/churches/photos/bytes/{firstPhoto.NamePhoto}";
                                    try
                                    {
                                        using (var stream = await client.GetStreamAsync(imageUrl))
                                        {
                                            var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
                                            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                                            {
                                                await stream.CopyToAsync(fileStream);
                                            }

                                            var bitmap = new Bitmap(tempFilePath);
                                            church.FirstPhoto = bitmap;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Unable to load bitmap for photo {firstPhoto.NamePhoto}: {ex.Message}");
                                    }
                                }
                            }

                            // Устанавливаем отображаемые значения
                            church.ChurchnameDisplay = church.GetChurchname(IsRussian);
                            church.BuildDateDisplay = church.GetBuildDate(IsRussian);
                        }
                        ChurchesListBox.ItemsSource = churches;
                    }
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                Console.WriteLine($"Error fetching churches: {httpRequestException.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new MapControl(IsRussian)); 
        }

        private void ChurchesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChurchesListBox.SelectedItem is AllDTO.ChurchDto selectedChurch)
            {
                NavigationManager.NavigateTo(new ChurchDetailControl(selectedChurch, Idregion, IsRussian));
            }
        }

        private void UpdateUIForLanguage()
        {
            var titleTextBlock = this.FindControl<TextBlock>("TitleTextBlock");
            var backButton = this.FindControl<Button>("BackButton");

            if (IsRussian)
            {
                titleTextBlock.Text = "Список церквей";
                backButton.Content = "Вернуться";
            }
            else
            {
                titleTextBlock.Text = "List of Churches";
                backButton.Content = "Back";
            }
        }
    }
}
