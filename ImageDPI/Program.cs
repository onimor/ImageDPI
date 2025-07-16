using System;
using System.Drawing;
using System.Drawing.Imaging;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Использование: ConvertDpi.exe <original_image> <translated_image> <output_image>");
            return;
        }

        string originalPath = args[0];
        string translatedPath = args[1];
        string outputPath = args[2];

        using (var originalImage = Image.FromFile(originalPath))
        using (var translatedImageRaw = Image.FromFile(translatedPath))
        {
            float origDpiX = originalImage.HorizontalResolution;
            float origWidthPx = originalImage.Width;
            float origWidthInch = origWidthPx / origDpiX;

            float translatedWidthPx = translatedImageRaw.Width;
            float translatedHeightPx = translatedImageRaw.Height;

            float newDpiX = translatedWidthPx / origWidthInch;
            float newDpiY = translatedHeightPx / (originalImage.Height / originalImage.VerticalResolution);
            
            Console.WriteLine($"Исходный DPI: {origDpiX}");
            Console.WriteLine($"Новый DPI: {newDpiX}");

            // Приводим к Bitmap и устанавливаем новый DPI
            using (var translatedBitmap = new Bitmap(translatedImageRaw))
            {
                translatedBitmap.SetResolution(newDpiX, newDpiY);
                translatedBitmap.Save(outputPath, ImageFormat.Png);
            }
        }

        Console.WriteLine("Готово. Сохранено в: " + outputPath);
    }
}
