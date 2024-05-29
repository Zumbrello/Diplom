using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Diplom.UserControls
{
    public partial class MapControl : UserControl
    {
        private readonly HttpClient _httpClient;
        private Dictionary<string, Image> _regionImages;
        private Dictionary<string, string> _regionImagePaths;
        private Dictionary<string, string> _regionImageGreenPaths;
        private Dictionary<string, Bitmap> _regionBitmaps;
        private string _currentHighlightedRegion;

        public MapControl()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5249/") };
            InitializeRegionImages();
            LoadRegions();
        }

        private void InitializeRegionImages()
        {
            _regionImages = new Dictionary<string, Image>
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

            foreach (var region in _regionImagePaths)
            {
                var bitmap = new Bitmap(region.Value);
                _regionBitmaps[region.Key] = bitmap;
            }

            foreach (var region in _regionImages)
            {
                region.Value.PointerMoved += Image_PointerMoved;
                region.Value.PointerExited += Image_PointerExited;
            }
        }

        private async void LoadRegions()
        {
            try
            {
                Console.WriteLine("Sending request to: http://localhost:5249/api/churches/regions");
                var regions = await _httpClient.GetFromJsonAsync<List<Region>>("api/churches/regions");
                foreach (var region in regions)
                {
                    if (_regionImages.ContainsKey(region.Nameofregion))
                    {
                        region.Tag = _regionImages[region.Nameofregion];
                    }
                }
                RegionsListBox.ItemsSource = regions;
            }
            catch (HttpRequestException httpRequestException)
            {
                Console.WriteLine($"Error fetching regions: {httpRequestException.Message}");
            }
        }

        private void HighlightRegion(string regionName)
        {
            if (_currentHighlightedRegion != null && _regionImages.ContainsKey(_currentHighlightedRegion))
            {
                _regionImages[_currentHighlightedRegion].Source = new Bitmap(_regionImagePaths[_currentHighlightedRegion]);
            }

            if (_regionImages.ContainsKey(regionName) && _regionImageGreenPaths.ContainsKey(regionName))
            {
                _regionImages[regionName].Source = new Bitmap(_regionImageGreenPaths[regionName]);
                _currentHighlightedRegion = regionName;
            }
        }

        private void ResetHighlight()
        {
            if (_currentHighlightedRegion != null && _regionImages.ContainsKey(_currentHighlightedRegion))
            {
                _regionImages[_currentHighlightedRegion].Source = new Bitmap(_regionImagePaths[_currentHighlightedRegion]);
                _currentHighlightedRegion = null;
            }
        }

        private void RegionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRegion = (Region)RegionsListBox.SelectedItem;
            if (selectedRegion != null)
            {
                HighlightRegion(selectedRegion.Nameofregion);
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
                if (region != null && region.Nameofregion != _currentHighlightedRegion)
                {
                    HighlightRegion(region.Nameofregion);
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

                foreach (var region in _regionImages)
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

                        var bitmap = _regionBitmaps[regionName];
                        var widthScale = bitmap.PixelSize.Width / image.Bounds.Width;
                        var heightScale = bitmap.PixelSize.Height / image.Bounds.Height;

                        var scaledX = (int)(localX * widthScale);
                        var scaledY = (int)(localY * heightScale);

                        if (IsPointInNonTransparentRegion(_regionImagePaths[regionName], scaledX, scaledY))
                        {
                            HighlightRegion(regionName);
                            isOverRegion = true;

                            // Показать Tooltip
                            RegionToolTip.Text = regionName;
                            RegionToolTip.IsVisible = true;
                            ToolTipBackground.IsVisible = true;
                            Canvas.SetLeft(RegionToolTip, point.X + 10); // сместите на 10 пикселей вправо от курсора
                            Canvas.SetTop(RegionToolTip, point.Y + 10);  // сместите на 10 пикселей вниз от курсора

                            Canvas.SetLeft(ToolTipBackground, point.X + 10); // сместите на 10 пикселей вправо от курсора
                            Canvas.SetTop(ToolTipBackground, point.Y + 10);  // сместите на 10 пикселей вниз от курсора
                            ToolTipBackground.Width = RegionToolTip.Bounds.Width + 10;
                            ToolTipBackground.Height = RegionToolTip.Bounds.Height + 10;
                            break;
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

                foreach (var region in _regionImages)
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

                        var bitmap = _regionBitmaps[regionName];
                        var widthScale = bitmap.PixelSize.Width / image.Bounds.Width;
                        var heightScale = bitmap.PixelSize.Height / image.Bounds.Height;

                        var scaledX = (int)(localX * widthScale);
                        var scaledY = (int)(localY * heightScale);

                        if (IsPointInNonTransparentRegion(_regionImagePaths[regionName], scaledX, scaledY))
                        {
                            HighlightRegion(regionName);
                            var selectedRegion = RegionsListBox.Items.OfType<Region>().FirstOrDefault(r => r.Nameofregion == regionName);
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

        private bool IsPointInNonTransparentRegion(string imagePath, int x, int y)
        {
            using (var stream = File.OpenRead(imagePath))
            {
                var bitmap = new Bitmap(stream);
                var width = bitmap.PixelSize.Width;
                var height = bitmap.PixelSize.Height;
                var stride = width * 4; // 4 bytes per pixel for BGRA format
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
            var image = sender as Image;
            if (image != null)
            {
                var regionName = _regionImages.FirstOrDefault(x => x.Value == image).Key;
                if (!string.IsNullOrEmpty(regionName))
                {
                    var point = e.GetPosition(image);
                    var bitmap = _regionBitmaps[regionName];
                    var widthScale = bitmap.PixelSize.Width / image.Bounds.Width;
                    var heightScale = bitmap.PixelSize.Height / image.Bounds.Height;

                    var scaledX = (int)(point.X * widthScale);
                    var scaledY = (int)(point.Y * heightScale);

                    if (IsPointInNonTransparentRegion(_regionImagePaths[regionName], scaledX, scaledY))
                    {
                        HighlightRegion(regionName);

                        RegionToolTip.Text = regionName;
                        RegionToolTip.IsVisible = true;
                        ToolTipBackground.IsVisible = true;

                        var canvasPoint = e.GetPosition(MainCanvas); // Используйте MainCanvas вместо image
                        Canvas.SetLeft(RegionToolTip, canvasPoint.X + 10); // сместите на 10 пикселей вправо от курсора
                        Canvas.SetTop(RegionToolTip, canvasPoint.Y + 10);  // сместите на 10 пикселей вниз от курсора

                        Canvas.SetLeft(ToolTipBackground, canvasPoint.X + 10); // сместите на 10 пикселей вправо от курсора
                        Canvas.SetTop(ToolTipBackground, canvasPoint.Y + 10);  // сместите на 10 пикселей вниз от курсора
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

        private void Image_PointerExited(object sender, PointerEventArgs e)
        {
            ResetHighlight();
            RegionToolTip.IsVisible = false;
            ToolTipBackground.IsVisible = false;
        }

        private void NavigateToRegion(int regionId)
        {
            ChurcesControl churcesControl = new ChurcesControl(regionId);
            NavigationManager.NavigateTo(churcesControl);
        }

        private void Menu_Click(object? sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new GameMenu());
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
    }

    public class Region 
    {
        public int Id { get; set; }
        public string Nameofregion { get; set; }
        public object Tag { get; set; }
    }
}