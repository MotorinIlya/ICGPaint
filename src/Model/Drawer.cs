using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace src.Model;


public class Drawer(UserControl drawingPanel, Color color)
{
    private UserControl _drawingPanel = drawingPanel;
    private Color _color = color;

    public void DrawPixel(WriteableBitmap bitmap, Point position)
    {
        int x = (int)position.X;
        int y = (int)position.Y;

        if (x >= 0 && x < bitmap.PixelSize.Width && y >= 0 && y < bitmap.PixelSize.Height)
        {
            using (var buffer = bitmap.Lock())
            {
                int offset = y * buffer.RowBytes + x * 4;

                unsafe
                {
                    byte* ptr = (byte*)buffer.Address.ToPointer();
                    ptr[offset] = _color.B;
                    ptr[offset + 1] = _color.G;
                    ptr[offset + 2] = _color.R;
                    ptr[offset + 3] = _color.A;
                }
            }

            _drawingPanel.InvalidateVisual();
        }
    }

    public void DrawLine(WriteableBitmap bitmap, Point start, Point end)
    {
        var x0 = (int)start.X;
        var y0 = (int)start.Y;
        var x1 = (int)end.X;
        var y1 = (int)end.Y;

        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = x0 < x1 ? 1 : -1;
        var sy = y0 < y1 ? 1 : -1;
        var err = dx - dy;

        while (true)
        {
            DrawPixel(bitmap, new Point(x0, y0));

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= 2 * dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += 2 * dx;
                y0 += sy;
            }
        }
    }
}