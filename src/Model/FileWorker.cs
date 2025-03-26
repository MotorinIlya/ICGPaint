using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using SkiaSharp;
using src.View;

namespace src.Model;

public class FileWorker
{
    public async Task SetImage(MainWindow window)
    {
        var bitmap = await LoadImageDirectlyAsync(window);
        if (bitmap != null)
        {
            window.Panel.SetBitmap(bitmap);
        }
    }

    public async Task SaveImage(MainWindow window)
    {
        await SaveImageAsync(window);
    }

    private async Task<WriteableBitmap?> LoadImageDirectlyAsync(MainWindow window)
    {
        var topLevel = TopLevel.GetTopLevel(window);
        if (topLevel is not null)
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Выберите изображение",
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });
            

            if (files.Count == 0 || files[0] is not IStorageFile file)
                return null;

            await using var stream = await file.OpenReadAsync();
            return await Task.Run(() =>
            {
                using var skiaStream = new SKManagedStream(stream);
                using var skiaBitmap = SKBitmap.Decode(skiaStream);

                if (skiaBitmap.ColorType != SKColorType.Bgra8888)
                {
                    skiaBitmap.CopyTo(skiaBitmap, SKColorType.Bgra8888);
                }

                using var oldBuffer = window.Panel.Bitmap.Lock();
                var width = Math.Max(oldBuffer.Size.Width, skiaBitmap.Width);
                var height = Math.Max(oldBuffer.Size.Height, skiaBitmap.Height);

                var writableBitmap = new WriteableBitmap(
                    new PixelSize(width, height),
                    new Vector(96, 96),
                    PixelFormat.Bgra8888
                );

                using var bitmapLock = writableBitmap.Lock();
                unsafe
                {
                    var src = (byte*)oldBuffer.Address.ToPointer();
                    var dst = (byte*)bitmapLock.Address.ToPointer();
                    
                    for (int y = 0; y < oldBuffer.Size.Height; y++)
                    {
                        Buffer.MemoryCopy(
                            src + y * oldBuffer.RowBytes,
                            dst + y * bitmapLock.RowBytes,
                            oldBuffer.Size.Width * 4,
                            oldBuffer.Size.Width * 4
                        );
                    }

                    var skia = (byte*)skiaBitmap.GetPixels().ToPointer();

                    for (var y = 0; y < skiaBitmap.Height; y++)
                    {
                        Buffer.MemoryCopy(
                            skia + y * skiaBitmap.RowBytes,
                            dst + y * bitmapLock.RowBytes,
                            skiaBitmap.Width * 4,
                            skiaBitmap.Width * 4
                        );
                    }
                }

                return writableBitmap;
            });
        }
        return null;
    }

    public async Task SaveImageAsync(MainWindow window)
    {
        var topLevel = TopLevel.GetTopLevel(window);
        if (topLevel is not null)
        {
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Сохранить изображение",
                FileTypeChoices = new[] 
                {
                    FilePickerFileTypes.ImagePng,
                    new FilePickerFileType("JPEG") { Patterns = new[] { "*.jpg", "*.jpeg" } }
                },
                DefaultExtension = ".png",
                ShowOverwritePrompt = true
            });

            if (file == null) return;

            using var skImage = ConvertToSkiaImage(window.Panel.Bitmap);
            if (skImage == null) return;

            await using var stream = await file.OpenWriteAsync();
            using var data = skImage.Encode(GetFormat(file.Name), 90);
            data.SaveTo(stream);
        }
    }

    private SKImage ConvertToSkiaImage(WriteableBitmap bitmap)
    {
        using var locked = bitmap.Lock();
        var info = new SKImageInfo(
            locked.Size.Width,
            locked.Size.Height,
            SKColorType.Bgra8888,
            SKAlphaType.Premul);

        return SKImage.FromPixelCopy(info, locked.Address);
    }

    private SKEncodedImageFormat GetFormat(string fileName)
    {
        return Path.GetExtension(fileName).ToLower() switch
        {
            ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
            _ => SKEncodedImageFormat.Png
        };
    }
}