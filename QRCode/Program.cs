using System;
using System.IO;
using QRCoder;
using SkiaSharp;

class Program
{
    static void Main(string[] args)
    {
        
        Console.Write("Enter the URL: ");
        string? url = Console.ReadLine();
        if (string.IsNullOrEmpty(url))
        {
            Console.WriteLine("URL cannot be empty. Exiting...");
            return;
        }

        Console.Write("Enter the software name: ");
        string? softwareName = Console.ReadLine();
        if (string.IsNullOrEmpty(softwareName))
        {
            Console.WriteLine("Software name cannot be empty. Exiting...");
            return;
        }

        // Generate the QR code using QRCoder
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        // Convert the QR code byte array to a SkiaSharp bitmap
        using var qrCodeBitmap = SKBitmap.Decode(qrCodeBytes);
        using var finalImage = AddTextAboveImage(qrCodeBitmap, softwareName);

        // Get the Downloads folder path
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        string filePath = Path.Combine(downloadsPath, softwareName + ".png");

        // Save the final image to the Downloads folder
        using var image = SKImage.FromBitmap(finalImage);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(filePath);
        data.SaveTo(stream);

        Console.WriteLine($"QR Code saved at: {filePath}");
    }

    // Method to add text above the QR code using SkiaSharp
    static SKBitmap AddTextAboveImage(SKBitmap qrCodeBitmap, string text)
    {
        int width = qrCodeBitmap.Width;
        int height = qrCodeBitmap.Height + 50; // Extra space for text
        var finalImage = new SKBitmap(width, height);

        using var canvas = new SKCanvas(finalImage);
        canvas.Clear(SKColors.White);

        // Define text style
        using var paint = new SKPaint
        {
            TextSize = 24,
            IsAntialias = true,
            Color = SKColors.Black,
            TextAlign = SKTextAlign.Center
        };

        // Draw the text at the top
        float x = width / 2;
        float y = 30;
        canvas.DrawText(text, x, y, paint);

        // Draw the QR code below the text
        canvas.DrawBitmap(qrCodeBitmap, 0, 50);

        return finalImage;
    }
}
