using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Diplom.UserControls
{
    public partial class puzzles : UserControl
    {
        private List<PuzzlePiece> _puzzlePieces = new List<PuzzlePiece>();
        private Random _random = new Random();
        private const int PuzzleSize = 4; // 4x4 pieces
        private const double Tolerance = 10.0; // Allowable tolerance for snapping
        private const int BorderPadding = 5; // Padding around the puzzle area
        private const string ApiUrl = "http://localhost:5249/api/churches/photos"; // API URL
        private const string BytesApiUrl = "http://localhost:5249/api/churches/photos/bytes/"; // API URL for image bytes

        public puzzles()
        {
            InitializeComponent();
             PuzzleCanvas = this.FindControl<Canvas>("PuzzleCanvas");
            PuzzleBorder = this.FindControl<Border>("PuzzleBorder");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
           
        }

        private async void OnLoadImageClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                var randomImageName = await GetRandomImageNameAsync(ApiUrl);
                if (!string.IsNullOrEmpty(randomImageName))
                {
                    Console.WriteLine($"Image Name: {randomImageName}"); // Отладочная информация
                    var bitmap = await LoadBitmapFromBytesAsync(BytesApiUrl + Uri.EscapeDataString(randomImageName));
                    if (bitmap != null)
                    {
                        LoadImage(bitmap);
                    }
                    else
                    {
                        ShowMessage("Failed to load the bitmap from the API.");
                    }
                }
                else
                {
                    ShowMessage("Failed to get a valid image name from the API.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading image: {ex.Message}");
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
                    Console.WriteLine($"API Response: {jsonResponse}"); // Отладочная информация

                    var photos = JsonConvert.DeserializeObject<List<Photo>>(jsonResponse);
                    if (photos != null && photos.Count > 0)
                    {
                        var randomPhoto = photos[_random.Next(photos.Count)];
                        return randomPhoto.NamePhoto;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting image name: {ex.Message}"); // Отладочная информация
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
                    Console.WriteLine($"Error loading bitmap from bytes: {ex.Message}"); // Отладочная информация
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

                    // Position pieces randomly within the puzzle canvas bounds, considering the border padding
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

            // Переместить кусок на верхний уровень
            SetZIndex(_draggingPiece, 1);
        }

        private void OnPiecePointerMoved(object sender, PointerEventArgs e)
        {
            if (_draggingPiece != null)
            {
                var position = e.GetPosition(PuzzleCanvas) - _draggingOffset;

                // Ensure the piece stays within the bounds of the puzzle area
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

                // Сбросить ZIndex для всех кусков после завершения перемещения
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

            // Ensure piece is within the puzzle border
            if (currentX < BorderPadding || currentX > PuzzleCanvas.Bounds.Width - piece.Width - BorderPadding ||
                currentY < BorderPadding || currentY > PuzzleCanvas.Bounds.Height - piece.Height - BorderPadding)
            {
                ShowMessage("Piece is out of bounds");
                return;
            }

            if (Math.Abs(currentX - pieceData.CorrectPosition.X) < Tolerance && Math.Abs(currentY - pieceData.CorrectPosition.Y) < Tolerance)
            {
                Canvas.SetLeft(piece, pieceData.CorrectPosition.X);
                Canvas.SetTop(piece, pieceData.CorrectPosition.Y);
                piece.IsHitTestVisible = false; // Disable further interaction with correctly placed pieces
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
            ShowMessage("Puzzle Completed!");
        }

        private void ShowMessage(string message)
        {
            var messageBox = new Window
            {
                Content = new TextBlock
                {
                    Text = message,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
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