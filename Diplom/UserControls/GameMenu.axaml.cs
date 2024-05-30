using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;

namespace Diplom.UserControls
{
    public partial class GameMenu : UserControl
    {
        private List<PuzzlePiece> _puzzlePieces = new List<PuzzlePiece>();
        private Random _random = new Random();
        private const int PuzzleSize = 4; // 4x4 форма
        private const double Tolerance = 10.0; 
        private const int BorderPadding = 5; 
        private const string ApiUrl = "http://http://194.146.242.26:6666/api/churches/photos"; // API URL
        private const string BytesApiUrl = "http://http://194.146.242.26:6666/api/churches/photos/bytes/"; // API URL for image bytes
        private bool IsRussian;

        public GameMenu(bool isRussian)
        {
            InitializeComponent();
            IsRussian = isRussian;
            PuzzleCanvas = this.FindControl<Canvas>("PuzzleCanvas");
            PuzzleBorder = this.FindControl<Border>("PuzzleBorder");
            ReferenceImage = this.FindControl<Image>("ReferenceImage");
            ReferencePanel = this.FindControl<StackPanel>("ReferencePanel");
            UpdateUIForLanguage();
        }

        private void Puzzles_click(object? sender, RoutedEventArgs e)
        {
            PuzzlesGame.IsVisible = true;
            ReferencePanel.IsVisible = true;
            LoadNewPuzzle();
        }

        private async void LoadNewPuzzle()
        {
            try
            {
                var randomImageName = await GetRandomImageNameAsync(ApiUrl);
                if (!string.IsNullOrEmpty(randomImageName))
                {
                    var bitmap = await LoadBitmapFromBytesAsync(BytesApiUrl + Uri.EscapeDataString(randomImageName));
                    if (bitmap != null)
                    {
                        LoadImage(bitmap);
                        ReferenceImage.Source = bitmap;
                    }
                    else
                    {
                        ShowMessage(IsRussian ? "Не удалось загрузить изображение с API." : "Failed to load the bitmap from the API.");
                    }
                }
                else
                {
                    ShowMessage(IsRussian ? "Не удалось получить допустимое имя изображения с API." : "Failed to get a valid image name from the API.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"{(IsRussian ? "Ошибка при загрузке изображения" : "Error loading image")}: {ex.Message}");
            }
        }

        private async Task<string> GetRandomImageNameAsync(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var photos = JsonConvert.DeserializeObject<List<Photo>>(jsonResponse);
                    if (photos != null && photos.Count > 0)
                    {
                        photos = photos.FindAll(photo => photo.NamePhoto != "zaglushka.png");

                        if (photos.Count == 0)
                        {
                            throw new Exception("No valid images found.");
                        }

                        var randomPhoto = photos[_random.Next(photos.Count)];
                        return randomPhoto.NamePhoto;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting image name: {ex.Message}");
                    throw;
                }
            }
        }

        private async Task<Bitmap> LoadBitmapFromBytesAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        return new Bitmap(ms);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading bitmap from bytes: {ex.Message}");
                    return null;
                }
            }
        }

        private void LoadImage(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new Exception("Failed to load the image.");
            }

            var pieceWidth = bitmap.PixelSize.Width / PuzzleSize;
            var pieceHeight = bitmap.PixelSize.Height / PuzzleSize;

            var canvasWidth = bitmap.PixelSize.Width + BorderPadding * 2;
            var canvasHeight = bitmap.PixelSize.Height + BorderPadding * 2;

            PuzzleCanvas.Width = canvasWidth;
            PuzzleCanvas.Height = canvasHeight;
            PuzzleBorder.Width = canvasWidth;
            PuzzleBorder.Height = canvasHeight;

            PuzzleCanvas.Children.Clear();
            _puzzlePieces.Clear();

            for (int y = 0; y < PuzzleSize; y++)
            {
                for (int x = 0; x < PuzzleSize; x++)
                {
                    var piece = new CroppedBitmap(bitmap, new Avalonia.PixelRect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight));
                    var image = new Image
                    {
                        Source = piece,
                        Width = pieceWidth,
                        Height = pieceHeight
                    };

                    var initialX = _random.Next(BorderPadding, Math.Max(BorderPadding + 1, (int)(canvasWidth - pieceWidth - BorderPadding)));
                    var initialY = _random.Next(BorderPadding, Math.Max(BorderPadding + 1, (int)(canvasHeight - pieceHeight - BorderPadding)));

                    Canvas.SetLeft(image, initialX);
                    Canvas.SetTop(image, initialY);

                    image.PointerPressed += OnPiecePointerPressed;
                    image.PointerMoved += OnPiecePointerMoved;
                    image.PointerReleased += OnPiecePointerReleased;

                    _puzzlePieces.Add(new PuzzlePiece
                    {
                        Image = image,
                        CorrectPosition = new Point(x * pieceWidth + BorderPadding, y * pieceHeight + BorderPadding)
                    });

                    PuzzleCanvas.Children.Add(image);
                }
            }
        }

        private Image _draggingPiece = null!;
        private Point _draggingOffset;

        private void OnPiecePointerPressed(object sender, PointerPressedEventArgs e)
        {
            _draggingPiece = sender as Image;
            if (_draggingPiece == null) return;

            _draggingOffset = e.GetPosition(_draggingPiece);

            SetZIndex(_draggingPiece, 1);
        }

        private void OnPiecePointerMoved(object sender, PointerEventArgs e)
        {
            if (_draggingPiece != null)
            {
                var position = e.GetPosition(PuzzleCanvas) - _draggingOffset;

                position = new Point(
                    Math.Clamp(position.X, BorderPadding, PuzzleCanvas.Bounds.Width - _draggingPiece.Bounds.Width - BorderPadding),
                    Math.Clamp(position.Y, BorderPadding, PuzzleCanvas.Bounds.Height - _draggingPiece.Bounds.Height - BorderPadding)
                );

                Canvas.SetLeft(_draggingPiece, (int)Math.Round(position.X));
                Canvas.SetTop(_draggingPiece, (int)Math.Round(position.Y));
            }
        }

        private void OnPiecePointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (_draggingPiece != null)
            {
                CheckPiecePosition(_draggingPiece);

                foreach (var piece in _puzzlePieces)
                {
                    SetZIndex(piece.Image, 0);
                }

                _draggingPiece = null;
            }
        }

        private void CheckPiecePosition(Image piece)
        {
            var pieceData = _puzzlePieces.Find(p => p.Image == piece);
            if (pieceData == null) return;

            var currentX = Canvas.GetLeft(piece);
            var currentY = Canvas.GetTop(piece);

            if (currentX < BorderPadding || currentX > PuzzleCanvas.Bounds.Width - piece.Width - BorderPadding ||
                currentY < BorderPadding || currentY > PuzzleCanvas.Bounds.Height - piece.Height - BorderPadding)
            {
                ShowMessage(IsRussian ? "Кусок вне границ" : "Piece is out of bounds");
                return;
            }

            if (Math.Abs(currentX - pieceData.CorrectPosition.X) < Tolerance && Math.Abs(currentY - pieceData.CorrectPosition.Y) < Tolerance)
            {
                Canvas.SetLeft(piece, pieceData.CorrectPosition.X);
                Canvas.SetTop(piece, pieceData.CorrectPosition.Y);
                piece.IsHitTestVisible = false;
            }

            CheckPuzzleCompletion();
        }

        private void CheckPuzzleCompletion()
        {
            foreach (var piece in _puzzlePieces)
            {
                var currentX = Canvas.GetLeft(piece.Image);
                var currentY = Canvas.GetTop(piece.Image);

                if (Math.Abs(currentX - piece.CorrectPosition.X) >= Tolerance || Math.Abs(currentY - piece.CorrectPosition.Y) >= Tolerance)
                {
                    return;
                }
            }

            OnPuzzleCompleted();
        }

        private void OnPuzzleCompleted()
        {
            ShowMessage(IsRussian ? "Поздравляем, пазл собран!" : "Congratulations, puzzle completed!");
        }

        private void ShowMessage(string message)
        {
            var messageBox = new Window
            {
                Background = new SolidColorBrush(Color.Parse("#FFF9EB")),
                Content = new Border
                {
                    Background = new SolidColorBrush(Color.Parse("#FFF9EB")),
                    Child = new TextBlock
                    {
                        Text = message,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    }
                },
                Width = 200,
                Height = 100
            };

            messageBox.ShowDialog((Window)this.VisualRoot);
        }

        private void SetZIndex(Control control, int zIndex)
        {
            control.ZIndex = zIndex;
        }

        private void OnResetImageClick(object? sender, RoutedEventArgs e)
        {
            if (ReferenceImage.Source != null)
            {
                LoadImage(ReferenceImage.Source as Bitmap);
            }
        }

        private void OnNewPuzzleClick(object? sender, RoutedEventArgs e)
        {
            LoadNewPuzzle();
        }

        private void OnBackButtonClick(object? sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new MapControl(IsRussian));
        }

        private void UpdateUIForLanguage()
        {
            var backButton = this.FindControl<Button>("BackButton");
            var menuTitle = this.FindControl<TextBlock>("MenuTitle");
            var puzzlesButton = this.FindControl<Button>("PuzzlesButton");
            var resetPuzzleButton = this.FindControl<Button>("ResetPuzzleButton");
            var newPuzzleButton = this.FindControl<Button>("NewPuzzleButton");

            backButton.Content = IsRussian ? "Вернуться к карте" : "Back to Map";
            menuTitle.Text = IsRussian ? "Меню игр" : "Games Menu";
            puzzlesButton.Content = IsRussian ? "Игра пазлы" : "Puzzle Game";
            resetPuzzleButton.Content = IsRussian ? "Сбросить пазл" : "Reset Puzzle";
            newPuzzleButton.Content = IsRussian ? "Новый пазл" : "New Puzzle";
        }
    }

    public class Photo
    {
        public int Id { get; set; }
        public string NamePhoto { get; set; } = string.Empty;
        public List<string> PhotoOfHrams { get; set; } = new List<string>();
    }

    public class PuzzlePiece
    {
        public Image Image { get; set; } = null!;
        public Point CorrectPosition { get; set; }
    }
}
