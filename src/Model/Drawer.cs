using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
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
        _panel.InvalidateVisual();
    }

    public void FillArea(WriteableBitmap bitmap, Point start)
    {
        var oldColor = GetColor(bitmap, start);
        if (oldColor == _color)
        {
            return;
        }

        var width = GetWidth(bitmap);
        var height = GetHeight(bitmap);
        var x = (int)start.X;
        var y = (int)start.Y;
        Stack<(int, int)> stack = new();
        stack.Push((x, y));

        while (stack.Count > 0)
        {
            (var currentX, var currentY) = stack.Pop();
            if (GetColor(bitmap, currentX, currentY) != oldColor)
            {
                continue;
            }

            var leftX = currentX;
            while (leftX >= 0 && GetColor(bitmap, leftX, currentY) == oldColor)
            {
                leftX--;
            }
            leftX++;

            var rightX = currentX;
            while (rightX < width && GetColor(bitmap, rightX, currentY) == oldColor)
            {
                rightX++;
            }
            rightX--;

            for (var i = leftX; i <= rightX; i++)
            {
                DrawPixel(bitmap, i, currentY);
            }

            if (currentY > 0)
            {
                AddSpansToStack(bitmap, stack, -1, leftX, rightX, currentY, oldColor);
            }
            if (currentY < height - 1)
            {
                AddSpansToStack(bitmap, stack, 1, leftX, rightX, currentY, oldColor);
            }
        }
        _panel.InvalidateVisual();
    }

    private void AddSpansToStack(WriteableBitmap bitmap, 
            Stack<(int, int)> stack, 
            int increment,
            int leftX,
            int rightX,
            int currentY,
            Color oldColor)
    {
        var haveSpan = false;
        for (var i = leftX; i <= rightX; i++)
        {
            if (!haveSpan && GetColor(bitmap, i, currentY + increment) == oldColor)
            {
                stack.Push((i, currentY + increment));
                haveSpan = true;
            }
            else if (GetColor(bitmap, i, currentY + increment) != oldColor)
            {
                haveSpan = false;
            }
        }
    }

    public void Clear(WriteableBitmap bitmap)
    {
        using var buffer = bitmap.Lock();

        unsafe
        {
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
    }

    private unsafe void DrawPixel(WriteableBitmap bitmap, int x, int y) => 
            DrawPixel(bitmap, new Point(x, y));

    private unsafe void DrawPixel(WriteableBitmap bitmap, Point position)
    {
        int x = (int)position.X;
        int y = (int)position.Y;

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

    private unsafe Color GetColor(WriteableBitmap bitmap, int x, int y) =>
            GetColor(bitmap, new Point(x, y));

    private unsafe Color GetColor(WriteableBitmap bitmap, Point position)
    {
        var x = (int)position.X;
        var y = (int)position.Y;
        using var buffer = bitmap.Lock();
        var ptr = (byte*)buffer.Address;
        var offset = y * buffer.RowBytes + x * 4;

        var b = ptr[offset];
        var g = ptr[offset + 1];
        var r = ptr[offset + 2];
        var a = ptr[offset + 3];

        return Color.FromArgb(a, r, g, b);
    }

    private unsafe int GetWidth(WriteableBitmap bitmap)
    {
        using var buffer = bitmap.Lock();
        return buffer.Size.Width;
    }

    private unsafe int GetHeight(WriteableBitmap bitmap)
    {
        using var buffer = bitmap.Lock();
        return buffer.Size.Height;
    }
}