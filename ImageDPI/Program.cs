using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static System.Console;

namespace MyExif
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                WriteLine("Использование: MyExif.exe <original_image> <translated_image>");
                ReadLine();
                return;
            }

            string path1 = args[0];
            string path2 = args[1];

            if (!File.Exists(path1) || !File.Exists(path2))
            {
                WriteLine("Один или оба указанных файла не существуют.");
                ReadLine();
                return;
            }

            try
            {
                using (Image img1 = Image.FromFile(path1))
                using (Image img2 = Image.FromFile(path2))
                {
                    bool isImg1Original = img1.Width >= img2.Width;
                    Image original = isImg1Original ? img1 : img2;
                    Image translated = isImg1Original ? img2 : img1;
                    string translatedPath = isImg1Original ? path2 : path1;

                    float origDpi = original.HorizontalResolution;
                    int origWidth = original.Width, origHeight = original.Height;
                    float origWidthMm = PixelsToMillimeters(origWidth, origDpi);
                    float origHeightMm = PixelsToMillimeters(origHeight, origDpi);

                    float transDpi = translated.HorizontalResolution;
                    int transWidth = translated.Width, transHeight = translated.Height;
                    float transWidthMm = PixelsToMillimeters(transWidth, transDpi);
                    float transHeightMm = PixelsToMillimeters(transHeight, transDpi);

                    WriteLine("\nОригинал:");
                    PrintImageInfo(origDpi, origWidth, origHeight, origWidthMm, origHeightMm);

                    WriteLine("\nПереведённое:");
                    PrintImageInfo(transDpi, transWidth, transHeight, transWidthMm, transHeightMm);

                    float newDpi = origDpi * transWidth / (float)origWidth;

                    string ext = Path.GetExtension(translatedPath).ToLower();
                    ImageFormat format = (ext == ".jpg" || ext == ".jpeg") ? ImageFormat.Jpeg : ImageFormat.Png;
                    string outPath = Path.Combine(
                        Path.GetDirectoryName(translatedPath),
                        Path.GetFileNameWithoutExtension(translatedPath) + "_fixed" + ext
                    );

                    SaveCorrectedImage(translated, newDpi, outPath, format);

                    WriteLine($"\nСохранено: \"{outPath}\"");
                    WriteLine($"DPI скорректирован: {newDpi:0.0} для совпадения с шириной оригинала ≈ {origWidthMm:0.00} мм");
                }
            }
            catch (UnauthorizedAccessException)
            {
                WriteLine("\nОшибка: нет доступа к файлу. Запусти от имени администратора?");
            }
            catch (IOException ex)
            {
                WriteLine($"\nОшибка ввода-вывода: {ex.Message}");
            }
            catch (Exception ex)
            {
                WriteLine($"\nНеизвестная ошибка: {ex.Message}");
            }

            ReadLine();
        }

        static void SaveCorrectedImage(Image source, float newDpi, string outPath, ImageFormat format)
        {
            using (var output = new Bitmap(source))
            {
                output.SetResolution(newDpi, newDpi);
                output.Save(outPath, format);
            }
        }

        static float PixelsToMillimeters(int pixels, float dpi)
        {
            return pixels / dpi * 25.4f;
        }

        static void PrintImageInfo(float dpi, int width, int height, float widthMm, float heightMm)
        {
            WriteLine($"DPI:        {dpi:0.0}");
            WriteLine($"Pixels:     {width} × {height}");
            WriteLine($"Size (мм):  {widthMm:0.00} × {heightMm:0.00}");
        }
    }
}
