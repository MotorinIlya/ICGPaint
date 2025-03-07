using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using System;

using src.Model.Tools;
using src.Model;

namespace src.View.Drawing;


public partial class DrawingPanel : UserControl
{
    private WriteableBitmap _bitmap;
    private ITool _tool;
    public WriteableBitmap Bitmap => _bitmap;
    public DrawingPanel()
    {
        _tool = new PencilTool(this, Colors.Black);
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

        PointerPressed += _tool.OnPointerPressed;
        PointerReleased += _tool.OnPointerReleased;
        PointerMoved += _tool.OnPointerMoved;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.DrawImage(_bitmap, new Rect(0, 0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height));
    }
}