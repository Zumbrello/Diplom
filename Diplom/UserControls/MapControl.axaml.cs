using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;

namespace Diplom.UserControls
{
    public partial class MapControl : UserControl
    {
        private readonly HttpClient _httpClient;
        private Dictionary<string, Image> _regionImagesRu;
        private Dictionary<string, Image> _regionImagesEn;
        private Dictionary<string, string> _regionImagePaths;
        private Dictionary<string, string> _regionImageGreenPaths;
        private Dictionary<string, string> _regionImagePathsEng;
        private Dictionary<string, string> _regionImageGreenPathsEng;
        private Dictionary<string, Bitmap> _regionBitmaps;
        private string _currentHighlightedRegion;
        private bool IsRussian = true;

        public MapControl(bool isRussian = true)
        {
            InitializeComponent();
            IsRussian = isRussian;
            _httpClient = new HttpClient { BaseAddress = new Uri("http://194.146.242.26:6666/") };
            InitializeRegionImages();
            LoadRegions();
            UpdateUIForLanguage();
        }

        private void InitializeRegionImages()
        {
            _regionImagesRu = new Dictionary<string, Image>
            {
                { "Ломоватский", this.FindControl<Image>("Image1") },
                { "Марденгский", this.FindControl<Image>("Image2") },
                { "Юдинский", this.FindControl<Image>("Image3") },
                { "Красавинский", this.FindControl<Image>("Image4") },
                { "Сусоловский", this.FindControl<Image>("Image5") },
                { "Опокский", this.FindControl<Image>("Image6") },
                { "Нижнеерогодский", this.FindControl<Image>("Image7") },
                { "Самотовинский", this.FindControl<Image>("Image8") },
                { "Трегубовский", this.FindControl<Image>("Image9") },
                { "Шемогодский", this.FindControl<Image>("Image10") },
                { "Парфёновский", this.FindControl<Image>("Image11") },
                { "Покровский", this.FindControl<Image>("Image12") },
                { "Викторовский", this.FindControl<Image>("Image13") },
                { "Стреленский", this.FindControl<Image>("Image14") },
                { "Нижнешарденгский", this.FindControl<Image>("Image15") },
                { "Усть-Алексеевский", this.FindControl<Image>("Image16") },
                { "Орловский", this.FindControl<Image>("Image17") },
                { "Теплогорский", this.FindControl<Image>("Image18") },
                { "Верхнешарденгский", this.FindControl<Image>("Image19") },
                { "Верхневарженский", this.FindControl<Image>("Image20") }
            };

            _regionImagesEn = new Dictionary<string, Image>
            {
                { "Lomovatsky", this.FindControl<Image>("Image1") },
                { "Mardengsky", this.FindControl<Image>("Image2") },
                { "Yudinsky", this.FindControl<Image>("Image3") },
                { "Krasavinsky", this.FindControl<Image>("Image4") },
                { "Susolovsky", this.FindControl<Image>("Image5") },
                { "Opoksky", this.FindControl<Image>("Image6") },
                { "Nizhneerogodsky", this.FindControl<Image>("Image7") },
                { "Samotovinsky", this.FindControl<Image>("Image8") },
                { "Tregubovsky", this.FindControl<Image>("Image9") },
                { "Shemogodsky", this.FindControl<Image>("Image10") },
                { "Parfenovsky", this.FindControl<Image>("Image11") },
                { "Pokrovsky", this.FindControl<Image>("Image12") },
                { "Viktorovsky", this.FindControl<Image>("Image13") },
                { "Strelensky", this.FindControl<Image>("Image14") },
                { "Nizhneshardengsky", this.FindControl<Image>("Image15") },
                { "Ust-Alekseevsky", this.FindControl<Image>("Image16") },
                { "Orlovsky", this.FindControl<Image>("Image17") },
                { "Teplogorsky", this.FindControl<Image>("Image18") },
                { "Verkhneshardengsky", this.FindControl<Image>("Image19") },
                { "Verkhnevarzhensky", this.FindControl<Image>("Image20") }
            };

            _regionImagePaths = new Dictionary<string, string>
            {
                { "Верхневарженский", "Assets/risunok20.png" },
                { "Верхнешарденгский", "Assets/risunok19.png" },
                { "Красавинский", "Assets/risunok4.png" },
                { "Юдинский", "Assets/risunok3.png" },
                { "Усть-Алексеевский", "Assets/risunok16.png" },
                { "Самотовинский", "Assets/risunokk8.png" },
                { "Трегубовский", "Assets/risunok9.png" },
                { "Теплогорский", "Assets/risunok18.png" },
                { "Марденгский", "Assets/risunok2.png" },
                { "Ломоватский", "Assets/risunok1.png" },
                { "Орловский", "Assets/risunok17.png" },
                { "Опокский", "Assets/risunok6.png" },
                { "Викторовский", "Assets/risunok13.png" },
                { "Нижнеерогодский", "Assets/risunok7.png" },
                { "Нижнешарденгский", "Assets/risunok15.png" },
                { "Парфёновский", "Assets/risunok11.png" },
                { "Покровский", "Assets/risunok12.png" },
                { "Стреленский", "Assets/risunok14.png" },
                { "Сусоловский", "Assets/risunok5.png" },
                { "Шемогодский", "Assets/risunok10.png" }
            };

            _regionImagePathsEng = new Dictionary<string, string>
            {
                { "Verkhnevarzhensky", "Assets/risunok20.png" },
                { "Verkhneshardengsky", "Assets/risunok19.png" },
                { "Krasavinsky", "Assets/risunok4.png" },
                { "Yudinsky", "Assets/risunok3.png" },
                { "Ust-Alekseevsky", "Assets/risunok16.png" },
                { "Samotovinsky", "Assets/risunokk8.png" },
                { "Tregubovsky", "Assets/risunok9.png" },
                { "Teplogorsky", "Assets/risunok18.png" },
                { "Mardengsky", "Assets/risunok2.png" },
                { "Lomovatsky", "Assets/risunok1.png" },
                { "Orlovsky", "Assets/risunok17.png" },
                { "Opoksky", "Assets/risunok6.png" },
                { "Viktorovsky", "Assets/risunok13.png" },
                { "Nizhneerogodsky", "Assets/risunok7.png" },
                { "Nizhneshardengsky", "Assets/risunok15.png" },
                { "Parfenovsky", "Assets/risunok11.png" },
                { "Pokrovsky", "Assets/risunok12.png" },
                { "Strelensky", "Assets/risunok14.png" },
                { "Susolovsky", "Assets/risunok5.png" },
                { "Shemogodsky", "Assets/risunok10.png" }
            };

            _regionImageGreenPathsEng = new Dictionary<string, string>
            {
                { "Verkhnevarzhensky", "Assets/risunok20_green.png" },
                { "Verkhneshardengsky", "Assets/risunok19_green.png" },
                { "Krasavinsky", "Assets/risunok4_green.png" },
                { "Yudinsky", "Assets/risunok3_green.png" },
                { "Ust-Alekseevsky", "Assets/risunok16_green.png" },
                { "Tregubovsky", "Assets/risunok9_green.png" },
                { "Teplogorsky", "Assets/risunok18_green.png" },
                { "Samotovinsky", "Assets/risunokk8_green.png" },
                { "Orlovsky", "Assets/risunok17_green.png" },
                { "Mardengsky", "Assets/risunok2_green.png" },
                { "Lomovatsky", "Assets/risunok1_green.png" },
                { "Opoksky", "Assets/risunok6_green.png" },
                { "Viktorovsky", "Assets/risunok13_green.png" },
                { "Nizhneerogodsky", "Assets/risunok7_green.png" },
                { "Nizhneshardengsky", "Assets/risunok15_green.png" },
                { "Parfenovsky", "Assets/risunok11_green.png" },
                { "Pokrovsky", "Assets/risunok12_green.png" },
                { "Strelensky", "Assets/risunok14_green.png" },
                { "Susolovsky", "Assets/risunok5_green.png" },
                { "Shemogodsky", "Assets/risunok10_green.png" }
            };

            _regionImageGreenPaths = new Dictionary<string, string>
            {
                { "Верхневарженский", "Assets/risunok20_green.png" },
                { "Верхнешарденгский", "Assets/risunok19_green.png" },
                { "Красавинский", "Assets/risunok4_green.png" },
                { "Юдинский", "Assets/risunok3_green.png" },
                { "Усть-Алексеевский", "Assets/risunok16_green.png" },
                { "Трегубовский", "Assets/risunok9_green.png" },
                { "Теплогорский", "Assets/risunok18_green.png" },
                { "Самотовинский", "Assets/risunokk8_green.png" },
                { "Орловский", "Assets/risunok17_green.png" },
                { "Марденгский", "Assets/risunok2_green.png" },
                { "Ломоватский", "Assets/risunok1_green.png" },
                { "Опокский", "Assets/risunok6_green.png" },
                { "Викторовский", "Assets/risunok13_green.png" },
                { "Нижнеерогодский", "Assets/risunok7_green.png" },
                { "Нижнешарденгский", "Assets/risunok15_green.png" },
                { "Парфёновский", "Assets/risunok11_green.png" },
                { "Покровский", "Assets/risunok12_green.png" },
                { "Стреленский", "Assets/risunok14_green.png" },
                { "Сусоловский", "Assets/risunok5_green.png" },
                { "Шемогодский", "Assets/risunok10_green.png" }
            };

            _regionBitmaps = new Dictionary<string, Bitmap>();

            foreach (var region in IsRussian ? _regionImagePaths : _regionImagePathsEng)
            {
                var bitmap = new Bitmap(region.Value);
                _regionBitmaps[region.Key] = bitmap;
            }

            foreach (var region in _regionImagesRu)
            {
                region.Value.PointerMoved += Image_PointerMoved;
                region.Value.PointerExited += Image_PointerExited;
            }

            foreach (var region in _regionImagesEn)
            {
                region.Value.PointerMoved += Image_PointerMoved;
                region.Value.PointerExited += Image_PointerExited;
            }
        }

        private async void LoadRegions()
        {
            try
            {
                var regions = await _httpClient.GetFromJsonAsync<List<Region>>("api/churches/regions");
                foreach (var region in regions)
                {
                    if (IsRussian)
                    {
                        if (_regionImagesRu.ContainsKey(region.Nameofregion))
                        {
                            region.Tag = _regionImagesRu[region.Nameofregion];
                        }
                    }
                    else
                    {
                        if (_regionImagesEn.ContainsKey(region.NameofregionEng))
                        {
                            region.Tag = _regionImagesEn[region.NameofregionEng];
                        }
                    }
                }
                UpdateRegionDisplayNames(regions);
                RegionsListBox.ItemsSource = regions;
            }
            catch (HttpRequestException httpRequestException)
            {
                Console.WriteLine($"Error fetching regions: {httpRequestException.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void UpdateRegionDisplayNames(List<Region> regions)
        {
            foreach (var region in regions)
            {
                region.Nameofregion = region.IsRussian(IsRussian);
            }
        }

        private void HighlightRegion(string regionName)
        {
            try
            {
                if (!string.IsNullOrEmpty(_currentHighlightedRegion))
                {
                    var previousDictionary = IsRussian ? _regionImagesRu : _regionImagesEn;
                    var previousPaths = IsRussian ? _regionImagePaths : _regionImagePathsEng;

                    if (previousDictionary.ContainsKey(_currentHighlightedRegion))
                    {
                        previousDictionary[_currentHighlightedRegion].Source = new Bitmap(previousPaths[_currentHighlightedRegion]);
                    }
                }

                if (!string.IsNullOrEmpty(regionName))
                {
                    var currentDictionary = IsRussian ? _regionImagesRu : _regionImagesEn;
                    var greenPaths = IsRussian ? _regionImageGreenPaths : _regionImageGreenPathsEng;

                    if (currentDictionary.ContainsKey(regionName) && greenPaths.ContainsKey(regionName))
                    {
                        currentDictionary[regionName].Source = new Bitmap(greenPaths[regionName]);
                        _currentHighlightedRegion = regionName;
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Key not found in dictionary: {ex.Message}");
            }
        }

        private void ResetHighlight()
        {
            try
            {
                if (!string.IsNullOrEmpty(_currentHighlightedRegion))
                {
                    var dictionary = IsRussian ? _regionImagesRu : _regionImagesEn;
                    var paths = IsRussian ? _regionImagePaths : _regionImagePathsEng;

                    if (dictionary.ContainsKey(_currentHighlightedRegion))
                    {
                        dictionary[_currentHighlightedRegion].Source = new Bitmap(paths[_currentHighlightedRegion]);
                    }

                    _currentHighlightedRegion = null;
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Key not found in dictionary: {ex.Message}");
            }
        }

        private void RegionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRegion = (Region)RegionsListBox.SelectedItem;
            if (selectedRegion != null)
            {
                var regionName = selectedRegion.Nameofregion;
                HighlightRegion(regionName);
                NavigateToRegion(selectedRegion.Id);
            }
        }

        private void RegionsListBox_PointerMoved(object sender, PointerEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            var point = e.GetPosition(stackPanel);
            var item = stackPanel?.GetVisualsAt(point).FirstOrDefault() as Control;

            if (item != null)
            {
                var region = item.DataContext as Region;
                if (region != null)
                {
                    var regionName = region.Nameofregion;
                    if (regionName != _currentHighlightedRegion)
                    {
                        HighlightRegion(regionName);
                    }
                }
            }
        }

        private void RegionsListBox_PointerLeave(object sender, PointerEventArgs e)
        {
            ResetHighlight();
        }

        private void Canvas_PointerMoved(object sender, PointerEventArgs e)
        {
            var canvas = sender as Canvas;
            if (canvas != null)
            {
                var point = e.GetPosition(canvas);
                bool isOverRegion = false;

                foreach (var region in (IsRussian ? _regionImagesRu : _regionImagesEn))
                {
                    var image = region.Value;
                    var regionName = region.Key;
                    var left = Canvas.GetLeft(image);
                    var top = Canvas.GetTop(image);

                    if (left <= point.X && point.X <= left + image.Bounds.Width &&
                        top <= point.Y && point.Y <= top + image.Bounds.Height)
                    {
                        var localX = point.X - left;
                        var localY = point.Y - top;

                        var paths = IsRussian ? _regionImagePaths : _regionImagePathsEng;
                        if (_regionBitmaps.ContainsKey(regionName) && paths.ContainsKey(regionName))
                        {
                            var bitmap = _regionBitmaps[regionName];
                            var widthScale = bitmap.PixelSize.Width / image.Bounds.Width;
                            var heightScale = bitmap.PixelSize.Height / image.Bounds.Height;

                            var scaledX = (int)(localX * widthScale);
                            var scaledY = (int)(localY * heightScale);

                            if (IsPointInNonTransparentRegion(paths[regionName], scaledX, scaledY))
                            {
                                HighlightRegion(regionName);
                                isOverRegion = true;

                                // Показать Tooltip
                                RegionToolTip.Text = regionName;
                                RegionToolTip.IsVisible = true;
                                ToolTipBackground.IsVisible = true;
                                Canvas.SetLeft(RegionToolTip, point.X + 10); // смещение на 10 пикселей вправо от курсора
                                Canvas.SetTop(RegionToolTip, point.Y + 10);  // смещение на 10 пикселей вниз от курсора

                                Canvas.SetLeft(ToolTipBackground, point.X + 10); // смещение на 10 пикселей вправо от курсора
                                Canvas.SetTop(ToolTipBackground, point.Y + 10);  // смещение на 10 пикселей вниз от курсора
                                ToolTipBackground.Width = RegionToolTip.Bounds.Width + 10;
                                ToolTipBackground.Height = RegionToolTip.Bounds.Height + 10;
                                break;
                            }
                        }
                    }
                }

                if (!isOverRegion)
                {
                    ResetHighlight();
                    RegionToolTip.IsVisible = false; // Скрыть Tooltip
                    ToolTipBackground.IsVisible = false; // Скрыть фон
                }
            }
        }

        private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var canvas = sender as Canvas;
            if (canvas != null)
            {
                var point = e.GetPosition(canvas);

                foreach (var region in (IsRussian ? _regionImagesRu : _regionImagesEn))
                {
                    var image = region.Value;
                    var regionName = region.Key;
                    var left = Canvas.GetLeft(image);
                    var top = Canvas.GetTop(image);

                    if (left <= point.X && point.X <= left + image.Bounds.Width &&
                        top <= point.Y && point.Y <= top + image.Bounds.Height)
                    {
                        var localX = point.X - left;
                        var localY = point.Y - top;

                        var paths = IsRussian ? _regionImagePaths : _regionImagePathsEng;
                        if (_regionBitmaps.ContainsKey(regionName) && paths.ContainsKey(regionName))
                        {
                            var bitmap = _regionBitmaps[regionName];
                            var widthScale = bitmap.PixelSize.Width / image.Bounds.Width;
                            var heightScale = bitmap.PixelSize.Height / image.Bounds.Height;

                            var scaledX = (int)(localX * widthScale);
                            var scaledY = (int)(localY * heightScale);

                            if (IsPointInNonTransparentRegion(paths[regionName], scaledX, scaledY))
                            {
                                HighlightRegion(regionName);
                                var selectedRegion = RegionsListBox.Items.OfType<Region>().FirstOrDefault(r => (IsRussian ? r.Nameofregion : r.NameofregionEng) == regionName);
                                if (selectedRegion != null)
                                {
                                    RegionsListBox.SelectedItem = selectedRegion;
                                    NavigateToRegion(selectedRegion.Id);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool IsPointInNonTransparentRegion(string imagePath, int x, int y)
        {
            using (var stream = File.OpenRead(imagePath))
            {
                var bitmap = new Bitmap(stream);
                var width = bitmap.PixelSize.Width;
                var height = bitmap.PixelSize.Height;
                var stride = width * 4; 
                var pixelData = new byte[height * stride];
                var handle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
                try
                {
                    bitmap.CopyPixels(new PixelRect(0, 0, width, height), handle.AddrOfPinnedObject(), pixelData.Length, stride);

                    if (x < 0 || x >= width || y < 0 || y >= height)
                    {
                        Console.WriteLine("Click coordinates are out of bounds.");
                        return false;
                    }

                    int index = (y * stride) + (x * 4);

                    byte alpha = pixelData[index + 3];

                    return alpha != 0;
                }
                finally
                {
                    if (handle.IsAllocated)
                        handle.Free();
                }
            }
        }

        private void Image_PointerMoved(object sender, PointerEventArgs e)
        {
            try
            {
                var image = sender as Image;
                if (image != null)
                {
                    var regionName = (IsRussian ? _regionImagesRu : _regionImagesEn).FirstOrDefault(x => x.Value == image).Key;
                    if (!string.IsNullOrEmpty(regionName))
                    {
                        var point = e.GetPosition(image);
                        var bitmap = _regionBitmaps[regionName];
                        var widthScale = bitmap.PixelSize.Width / image.Bounds.Width;
                        var heightScale = bitmap.PixelSize.Height / image.Bounds.Height;

                        var scaledX = (int)(point.X * widthScale);
                        var scaledY = (int)(point.Y * heightScale);

                        var paths = IsRussian ? _regionImagePaths : _regionImagePathsEng;
                        if (IsPointInNonTransparentRegion(paths[regionName], scaledX, scaledY))
                        {
                            HighlightRegion(regionName);

                            RegionToolTip.Text = regionName;
                            RegionToolTip.IsVisible = true;
                            ToolTipBackground.IsVisible = true;

                            var canvasPoint = e.GetPosition(MainCanvas); 
                            Canvas.SetLeft(RegionToolTip, canvasPoint.X + 10); // смещение на 10 пикселей вправо от курсора
                            Canvas.SetTop(RegionToolTip, canvasPoint.Y + 10);  // смещение на 10 пикселей вниз от курсора

                            Canvas.SetLeft(ToolTipBackground, canvasPoint.X + 10); // смещение на 10 пикселей вправо от курсора
                            Canvas.SetTop(ToolTipBackground, canvasPoint.Y + 10);  // смещение на 10 пикселей вниз от курсора
                            ToolTipBackground.Width = RegionToolTip.Bounds.Width + 10;
                            ToolTipBackground.Height = RegionToolTip.Bounds.Height + 10;
                        }
                        else
                        {
                            RegionToolTip.IsVisible = false;
                            ToolTipBackground.IsVisible = false;
                        }
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Key not found in dictionary: {ex.Message}");
            }
        }

        private void Image_PointerExited(object sender, PointerEventArgs e)
        {
            ResetHighlight();
            RegionToolTip.IsVisible = false;
            ToolTipBackground.IsVisible = false;
        }

        private void NavigateToRegion(int regionId)
        {
            var churchesControl = new ChurchesControl(regionId, IsRussian);
            NavigationManager.NavigateTo(churchesControl);
        }

        private void Menu_Click(object? sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new GameMenu(IsRussian));
        }

        private void ExitButton_Click(object? sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Info_PointerEnter(object sender, PointerEventArgs e)
        {
            InfoToolTip.IsOpen = true;
        }

        private void Info_PointerLeave(object sender, PointerEventArgs e)
        {
            InfoToolTip.IsOpen = false;
        }

        private void SwitchToRussian_Click(object? sender, RoutedEventArgs e)
        {
            IsRussian = true;
            UpdateUIForLanguage();
            LoadRegions();
        }

        private void SwitchToEnglish_Click(object? sender, RoutedEventArgs e)
        {
            IsRussian = false;
            UpdateUIForLanguage();
            LoadRegions();
        }

        private void UpdateUIForLanguage()
        {
            this.DataContext = new
            {
                MenuButtonContent = IsRussian ? "Меню с играми и тестами" : "Games and Tests Menu",
                RegionsListLabel = IsRussian ? "Список районов:" : "List of Regions:",
                ExitButtonContent = IsRussian ? "Выход" : "Exit",
                MapTitle = IsRussian ? "Карта Великоустюгского округа" : "Map of Veliky Ustyug District",
                InfoToolTipText = IsRussian ? "Данное приложение разработано по книге С. А. Мальцева 'Храмы и часовни Великоустюгского округа'" : "This application is developed based on the book by S. A. Maltsev 'Churches and Chapels of Veliky Ustyug District'"
            };

            InitializeRegionImages(); // Обновляем изображения для текущего языка

            foreach (var region in (IsRussian ? _regionImagesRu : _regionImagesEn))
            {
                region.Value.PointerMoved += Image_PointerMoved;
                region.Value.PointerExited += Image_PointerExited;
            }

            if (RegionsListBox.ItemsSource is List<Region> regions)
            {
                UpdateRegionDisplayNames(regions);
                RegionsListBox.ItemsSource = null;
                RegionsListBox.ItemsSource = regions;
            }
        }
    }
}
