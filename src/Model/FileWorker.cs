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
        var bitmapTask = await LoadImageDirectlyAsync(window);
        window.Panel.SetBitmap(bitmapTask);
    }
    
    public async Task<WriteableBitmap?> LoadImageDirectlyAsync(Window window)
    {
        var topLevel = TopLevel.GetTopLevel(window);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите изображение",
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
        });

        if (files.Count == 0 || !(files[0] is IStorageFile file))
            return null;

        await using var stream = await file.OpenReadAsync();
        return await Task.Run(() =>
        {
            // Декодируем через SkiaSharp
            using var skiaStream = new SKManagedStream(stream);
            using var skiaBitmap = SKBitmap.Decode(skiaStream);

            if (skiaBitmap == null)
                return null;

            // Конвертируем формат пикселей в Bgra8888 (если нужно)
            if (skiaBitmap.ColorType != SKColorType.Bgra8888)
            {
                skiaBitmap.CopyTo(skiaBitmap, SKColorType.Bgra8888);
            }

            // Создаём WriteableBitmap
            var writableBitmap = new WriteableBitmap(
                new PixelSize(skiaBitmap.Width, skiaBitmap.Height),
                new Vector(96, 96),
                PixelFormat.Bgra8888
            );

            // Копируем данные через небезопасный код
            using (var bitmapLock = writableBitmap.Lock())
            {
                unsafe
                {
                    var srcPtr = (byte*)skiaBitmap.GetPixels().ToPointer();
                    var dstPtr = (byte*)bitmapLock.Address.ToPointer();
                    
                    Buffer.MemoryCopy(
                        srcPtr,
                        dstPtr,
                        bitmapLock.RowBytes * bitmapLock.Size.Height,
                        skiaBitmap.ByteCount
                    );
                }
            }

            return writableBitmap;
        });
    }
}