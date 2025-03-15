using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using src.View.Drawing;

namespace src.Model;


public class Drawer(DrawingPanel drawingPanel)
{
    private Color _color = Colors.Black;
    private int _thinkness;
    private DrawingPanel _panel = drawingPanel;

    public void DrawLine(WriteableBitmap bitmap, Point start, Point end)
    {
        var x0 = (int)start.X;
        var y0 = (int)start.Y;
        var x1 = (int)end.X;
        var y1 = (int)end.Y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int error;

        if (dx > dy)
        {
            error = -dx;
            while (x0 != x1)
            {
                DrawPixel(bitmap, x0, y0);
                if (error >= 0)
                {
                    y0 += sy;
                    error -= 2 * dx;
                }
                error += 2 * dy;
                x0 += sx;
            }
        }
        else
        {
            error = - dy;
            while (y0 != y1)
            {
                DrawPixel(bitmap, x0, y0);
                if (error >= 0)
                {
                    x0 += sx;
                    error -= 2 * dy;
                }
                error += 2 * dx;
                y0 += sy;
            }
        }
        DrawPixel(bitmap, x0, y0);
        _panel.InvalidateVisual();
    }

    public unsafe void FillArea(WriteableBitmap bitmap, Point start)
    {
        using var buffer = bitmap.Lock();
        var oldColor = GetColor(buffer, (int)start.X, (int)start.Y);
        if (oldColor == _color)
        {
            return;
        }

        var width = buffer.Size.Width;
        var height = buffer.Size.Height;
        var stride = buffer.RowBytes;
        var x = (int)start.X;
        var y = (int)start.Y;

        var queue = new Queue<(int, int)>();
        queue.Enqueue((x, y));

        var ptr = (byte*)buffer.Address.ToPointer();

        while (queue.Count > 0)
        {
            (var currentX, var currentY) = queue.Dequeue();
            int offset = currentY * stride + currentX * 4;
            if (!ColorEquals(ptr, offset, oldColor))
            {
                continue;
            }

            (var leftX, var rightX) = GetSpan(buffer, currentX, currentY, oldColor);

            for (var i = leftX; i <= rightX; i++)
            {
                DrawPixel(buffer, i, currentY);
            }

            if (currentY > 0)
            {
                AddSpansToStack(buffer, queue, -1, leftX, rightX, currentY, oldColor);
            }
            if (currentY < height - 1)
            {
                AddSpansToStack(buffer, queue, 1, leftX, rightX, currentY, oldColor);
            }
        }
        _panel.InvalidateVisual();
    }

    public unsafe void Clear(WriteableBitmap bitmap)
    {
        using var buffer = bitmap.Lock();

        byte* ptr = (byte*)buffer.Address;

        int width = buffer.Size.Width;
        int height = buffer.Size.Height;
        int stride = buffer.RowBytes;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * stride + x * 4;

                ptr[index + 0] = 255;
                ptr[index + 1] = 255;
                ptr[index + 2] = 255;
                ptr[index + 3] = 255;
            }
        }
    }

    private unsafe void DrawPixel(WriteableBitmap bitmap, int x, int y)
    {
        if (x >= 0 && x < bitmap.PixelSize.Width && y >= 0 && y < bitmap.PixelSize.Height)
        {
            using var buffer = bitmap.Lock();
            int offset = y * buffer.RowBytes + x * 4;
            byte* ptr = (byte*)buffer.Address.ToPointer();

            ptr[offset] = _color.B;
            ptr[offset + 1] = _color.G;
            ptr[offset + 2] = _color.R;
            ptr[offset + 3] = _color.A;
        }
    }

    private unsafe int GetRowBytes(WriteableBitmap bitmap)
    {
        using var buffer = bitmap.Lock();
        return buffer.RowBytes;
    }

    public void DrawPolygon(WriteableBitmap bitmap, 
                            Point center, 
                            int countAngle, 
                            int measureAngle,
                            int radius)
    {
        var rotation = measureAngle * Math.PI / 180;
        var points = new Point[countAngle];
        for (var i = 0; i < countAngle; i++)
        {
            var angle = 2 * Math.PI * i / countAngle + rotation;
            points[i] = new Point(
                center.X + radius * Math.Cos(angle),
                center.Y + radius * Math.Sin(angle)
            );
        }

        var stride = GetRowBytes(bitmap);

        for (var i = 0; i < points.Length; i++)
        {
            var start = points[i];
            var end = points[(i + 1) % points.Length];
            DrawLine(bitmap, start, end);
        }

        _panel.InvalidateVisual();
    }


    //--------------------------------------------

    private unsafe void DrawPixel(ILockedFramebuffer buffer, int x, int y)
    {
        if (x >= 0 && x < buffer.Size.Width && y >= 0 && y < buffer.Size.Height)
        {
            var offset = y * buffer.RowBytes + x * 4;
            var ptr = (byte*)buffer.Address.ToPointer();

            ptr[offset] = _color.B;
            ptr[offset + 1] = _color.G;
            ptr[offset + 2] = _color.R;
            ptr[offset + 3] = _color.A;
        }
    }

    private unsafe Color GetColor(ILockedFramebuffer buffer, int x, int y)
    {
        var ptr = (byte*)buffer.Address.ToPointer();
        var offset = y * buffer.RowBytes + x * 4;

        var b = ptr[offset];
        var g = ptr[offset + 1];
        var r = ptr[offset + 2];
        var a = ptr[offset + 3];

        return Color.FromArgb(a, r, g, b);
    }

    private unsafe void AddSpansToStack(ILockedFramebuffer buffer, 
            Queue<(int, int)> queue, 
            int increment,
            int leftX,
            int rightX,
            int currentY,
            Color oldColor)
    {
        var haveSpan = false;
        var ptr = (byte*)buffer.Address.ToPointer();
        var stride = buffer.RowBytes;
        for (var i = leftX; i <= rightX; i++)
        {
            if (!haveSpan && ColorEquals(ptr, (currentY + increment) * stride + i * 4, oldColor))
            {
                queue.Enqueue((i, currentY + increment));
                haveSpan = true;
            }
            else if (!ColorEquals(ptr, (currentY + increment) * stride + i * 4, oldColor))
            {
                haveSpan = false;
            }
        }
    }

    private unsafe bool ColorEquals(byte* ptr, int offset, Color color)
    {
        return ptr[offset] == color.B
                && ptr[offset + 1] == color.G
                && ptr[offset + 2] == color.R
                && ptr[offset + 3] == color.A;
    }

    private unsafe (int, int) GetSpan(ILockedFramebuffer buffer, 
                    int x, int y,  
                    Color oldColor)
    {
        var ptr = (byte*)buffer.Address.ToPointer();
        var stride = buffer.RowBytes;
        var width = buffer.Size.Width;

        var leftX = x;
        while (leftX >= 0 && ColorEquals(ptr, y * stride + leftX * 4, oldColor))
        {
            leftX--;
        }
        leftX++;

        var rightX = x;
        while (rightX < width && ColorEquals(ptr, y * stride + rightX * 4, oldColor))
        {
            rightX++;
        }
        rightX--;

        return (leftX, rightX);
    }
}