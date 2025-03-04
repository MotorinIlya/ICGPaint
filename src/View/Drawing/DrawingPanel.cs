using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;

namespace src.View.Drawing;


public partial class DrawingPanel : UserControl
{
    private WriteableBitmap _bitmap;
    private bool _isDrawing = false;

    public DrawingPanel()
    {
        _bitmap = new(
            new PixelSize(1920, 1080),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);

        using (var buffer = _bitmap.Lock())
        {
            unsafe
            {
                byte* ptr = (byte*)buffer.Address.ToPointer();
                for (int i = 0; i < buffer.Size.Width * buffer.Size.Height * 4; i += 4)
                {
                    ptr[i] = 255;
                    ptr[i + 1] = 255;
                    ptr[i + 2] = 255;
                    ptr[i + 3] = 255;
                }
            }
        }

        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
        PointerMoved += OnPointerMoved;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.DrawImage(_bitmap, new Rect(0, 0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height));
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        _isDrawing = true;
        DrawPixel(e.GetPosition(this), Colors.Red);
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        _isDrawing = false;
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (_isDrawing)
        {
            DrawPixel(e.GetPosition(this), Colors.Red);
        }
    }

    private void DrawPixel(Point position, Color color)
    {
        int x = (int)position.X;
        int y = (int)position.Y;

        if (x >= 0 && x < _bitmap.PixelSize.Width && y >= 0 && y < _bitmap.PixelSize.Height)
        {
            using (var buffer = _bitmap.Lock())
            {
                int offset = y * buffer.RowBytes + x * 4;

                unsafe
                {
                    byte* ptr = (byte*)buffer.Address.ToPointer();
                    ptr[offset] = color.B;
                    ptr[offset + 1] = color.G;
                    ptr[offset + 2] = color.R;
                    ptr[offset + 3] = color.A;
                }
            }

            InvalidateVisual();
        }
    }
}