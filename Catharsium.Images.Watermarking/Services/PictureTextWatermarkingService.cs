using Catharsium.Images.Watermarking.Helpers;
using Catharsium.Images.Watermarking.Models;
using SkiaSharp;

namespace Catharsium.Images.Watermarking.Services;

public class PictureTextWatermarkingService : WatermarkingService<string>
{
    public override SKBitmap ApplyTo(SKBitmap picture, WatermarkRequest<string> request, bool useGrayScale)
    {
        using var canvas = new SKCanvas(picture);

        using var paint = new SKPaint
        {
            Color = new SKColor(173, 216, 230, (byte)(0.5f * 255)), // LightBlue with 50% alpha
            IsAntialias = true
        };

        using var font = new SKFont
        {
            Size = 80
        };

        var bounds = new SKRect();
        font.MeasureText(request.Image, out bounds);

        var watermarkWidth = (int)(picture.Width * request.Scale);
        var watermarkHeight = (int)(watermarkWidth / (double)bounds.Width * bounds.Height);

        if (picture.Height > picture.Width)
        {
            var factor = picture.Height / (double)picture.Width;
            watermarkWidth = (int)(watermarkWidth * factor);
            watermarkHeight = (int)(watermarkHeight * factor);
        }

        (var x, var y) = Position.GetCoordinates(
            request.Anchor,
            picture.Width,
            picture.Height,
            watermarkWidth,
            watermarkHeight,
            request.OffsetX,
            request.OffsetY);

        var scale = watermarkHeight / bounds.Height;
        font.Size *= (float)scale;

        canvas.DrawText(request.Image, x, y + watermarkHeight, font, paint);

        canvas.Flush();

        return picture;
    }
}