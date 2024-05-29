using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Interactivity;

namespace Diplom.UserControls
{
    public partial class ChurchDetailControl : UserControl, INotifyPropertyChanged
    {
        private AllDTO.ChurchDto _church;
        private ObservableCollection<ImageViewModel> _photos;
        private int _regionId;

        public event PropertyChangedEventHandler PropertyChanged;

        public ChurchDetailControl(AllDTO.ChurchDto church, int regionId)
        {
            InitializeComponent();
            _church = church;
            _regionId = regionId;
            _photos = new ObservableCollection<ImageViewModel>();

            this.FindControl<TextBlock>("ChurchNameTextBlock").Text = _church.Churchname;
            this.FindControl<TextBlock>("DescriptionTextBlock").Text = _church.Description;
            this.FindControl<ItemsControl>("PhotosItemsControl").ItemsSource = _photos;

            LoadChurchDetails();
        }

        public string ChurchName => _church?.Churchname;
        public string Description => _church?.Description;

        public ObservableCollection<ImageViewModel> Photos
        {
            get => _photos;
            set
            {
                _photos = value;
                OnPropertyChanged(nameof(Photos));
            }
        }

        private async void LoadChurchDetails()
        {
            OnPropertyChanged(nameof(ChurchName));
            OnPropertyChanged(nameof(Description));

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5249/");
                HttpResponseMessage photoResponse = await client.GetAsync($"api/churches/{_church.Id}/photos");
                if (photoResponse.IsSuccessStatusCode)
                {
                    var photos = await photoResponse.Content.ReadFromJsonAsync<List<AllDTO.PhotoDto>>();
                    if (photos != null && photos.Count > 0)
                    {
                        foreach (var photo in photos)
                        {
                            var imageUrl = $"http://localhost:5249/api/churches/photos/bytes/{photo.NamePhoto}";
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
                                    _photos.Add(new ImageViewModel { ImageSource = bitmap });
                                    Console.WriteLine($"Successfully loaded and saved photo {photo.NamePhoto} for church {_church.Id}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Unable to load bitmap for photo {photo.NamePhoto}: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No photos found for church {_church.Id}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch photos for church {_church.Id}: {photoResponse.StatusCode}");
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new ChurcesControl(_regionId));
        }

        public class ImageViewModel
        {
            public Bitmap ImageSource { get; set; }
        }
        
        private void OnMapButtonClick(object? sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new MapControl());
        }
    }
}