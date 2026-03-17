using Catharsium.Images.Watermarking.Helpers;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Files.Interfaces;
using SkiaSharp;

namespace Catharsium.Images.Watermarking.Services;

public class PictureImageWatermarkingService(IConsole console) : WatermarkingService<IFile>
{
    public override SKBitmap ApplyTo(SKBitmap picture, WatermarkRequest<IFile> request, bool useGrayScale)
    {
        console.WriteLine($"\tApplying {request.Image.Name}: {request}");

        using var watermarkStream = request.Image.OpenRead();
        using var watermarkBitmap = SKBitmap.Decode(watermarkStream);

        var watermarkWidth = (int)(picture.Width * request.Scale);
        var watermarkHeight = (int)(watermarkWidth / (double)watermarkBitmap.Width * watermarkBitmap.Height);

        if (picture.Height > picture.Width)
        {
            var factor = picture.Height / (double)picture.Width;
            watermarkWidth = (int)(watermarkWidth * factor);
            watermarkHeight = (int)(watermarkHeight * factor);
        }

        var width = picture.Width + this.LimitsX[request.Anchor];
        var height = picture.Height + this.LimitsY[request.Anchor];

        (var x, var y) = Position.GetCoordinates(request.Anchor, width, height, watermarkWidth, watermarkHeight, request.OffsetX, request.OffsetY);

        if (request.Flow == FlowDirection.Left)
            this.LimitsX[request.Anchor] -= width - x;

        if (request.Flow == FlowDirection.Right)
            this.LimitsX[request.Anchor] += width - x;

        if (request.Flow == FlowDirection.Up)
            this.LimitsY[request.Anchor] -= height - y;

        if (request.Flow == FlowDirection.Down)
            this.LimitsY[request.Anchor] += height - y;

        using var canvas = new SKCanvas(picture);

        if (request.BackgroundMarginX.HasValue && request.BackgroundMarginY.HasValue)
        {
            AddWatermarkBackground(picture, request, watermarkWidth, watermarkHeight, x, canvas);
        }

        SKPaint imagePaint = null;

        if (useGrayScale)
        {
            float[] matrix =            {
                0.3f, 0.3f, 0.3f, 0, 0,
                0.59f,0.59f,0.59f,0,0,
                0.11f,0.11f,0.11f,0,0,
                0,0,0,1,0
            };

            imagePaint = new SKPaint
            {
                ColorFilter = SKColorFilter.CreateColorMatrix(matrix),

                FilterQuality = SKFilterQuality.High
            };
        }
        else
        {
            imagePaint = new SKPaint
            {
                FilterQuality = SKFilterQuality.High
            };
        }

        console.WriteLine($"\t    - Drawing watermark {x}, {y}, {watermarkWidth}, {watermarkHeight}");
        var destRect = new SKRect(x, y, x + watermarkWidth, y + watermarkHeight);
        canvas.DrawBitmap(watermarkBitmap, destRect, imagePaint);
        canvas.Flush();

        return picture;
    }


    private void AddWatermarkBackground<T>(SKBitmap picture, WatermarkRequest<T> request, int watermarkWidth, int watermarkHeight, int x, SKCanvas canvas)
    {
        var backgroundY = picture.Height - watermarkHeight - (int)Math.Round(request.BackgroundMarginY.Value * watermarkHeight);
        var backgroundWidth = watermarkWidth + x + (int)Math.Round(request.BackgroundMarginX.Value * watermarkWidth);

        console.WriteLine($"\t    - Adding background 0, {backgroundY}, {backgroundWidth}, {picture.Height}");

        using var paint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 120),
            IsAntialias = true
        };

        canvas.DrawRect(new SKRect(0, backgroundY, backgroundWidth, picture.Height), paint);
    }
}