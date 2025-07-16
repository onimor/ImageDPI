using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Использование: MyExif.exe <original_image> <translated_image>");
            Console.ReadLine();
            return;
        }

        string path1 = args[0];
        string path2 = args[1];

        if (!File.Exists(path1) || !File.Exists(path2))
        {
            Console.WriteLine("Один или оба указанных файла не существуют.");
            Console.ReadLine();
            return;
        }

        try
        {
            using (Image img1 = Image.FromFile(path1))
            using (Image img2 = Image.FromFile(path2))
            {
                Image originalImage, translatedImage;
                string originalPath, translatedPath;

                // Определим оригинал по размеру
                if (img1.Width >= img2.Width)
                {
                    originalImage = img1;
                    translatedImage = img2;
                    originalPath = path1;
                    translatedPath = path2;
                }
                else
                {
                    originalImage = img2;
                    translatedImage = img1;
                    originalPath = path2;
                    translatedPath = path1;
                }

                // Исходные параметры
                float origDpiX = originalImage.HorizontalResolution;
                int origWidthPx = originalImage.Width;
                int origHeightPx = originalImage.Height;
                float origWidthMm = origWidthPx / origDpiX * 25.4f;
                float origHeightMm = origHeightPx / origDpiX * 25.4f;

                float transDpiX = translatedImage.HorizontalResolution;
                int transWidthPx = translatedImage.Width;
                int transHeightPx = translatedImage.Height;
                float transWidthMm = transWidthPx / transDpiX * 25.4f;
                float transHeightMm = transHeightPx / transDpiX * 25.4f;

                Console.WriteLine("\n Original image:");
                Console.WriteLine($"DPI     = {origDpiX:0.0}");
                Console.WriteLine($"Pixels  = {origWidthPx} x {origHeightPx}");
                Console.WriteLine($"Physical size = {origWidthMm:0.00} × {origHeightMm:0.00} mm");

                Console.WriteLine("\n Translated image:");
                Console.WriteLine($"DPI     = {transDpiX:0.0}");
                Console.WriteLine($"Pixels  = {transWidthPx} x {transHeightPx}");
                Console.WriteLine($"Physical size = {transWidthMm:0.00} × {transHeightMm:0.00} mm");

                // Расчёт нового DPI
                float newDpi = origDpiX * transWidthPx / origWidthPx;

                // Путь сохранения
                string outDir = Path.GetDirectoryName(translatedPath);
                string outName = Path.GetFileNameWithoutExtension(translatedPath) + "_fixed";
                string outExt = ".png"; // Если важна точность и метаданные, лучше сохранять как PNG
                string outputPath = Path.Combine(outDir, outName + outExt);

                using (var outputBitmap = new Bitmap(translatedImage))
                {
                    outputBitmap.SetResolution(newDpi, newDpi);
                    outputBitmap.Save(outputPath, ImageFormat.Png);
                }

                Console.WriteLine($"\n Сохранено исправленное изображение: \"{outputPath}\"");
                Console.WriteLine($"DPI скорректирован: {newDpi:0.0}, физ. ширина ≈ {origWidthMm:0.00} мм");

                Console.ReadLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n Ошибка: {ex.Message}");
            Console.ReadLine();
        }
    }
}
