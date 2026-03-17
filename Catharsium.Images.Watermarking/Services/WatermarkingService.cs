using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;
using SkiaSharp;

namespace Catharsium.Images.Watermarking.Services;

public abstract class WatermarkingService<T> : IWatermarkingService<T>
{
    protected Dictionary<Anchor, int> LimitsX { get; set; }
    protected Dictionary<Anchor, int> LimitsY { get; set; }
    protected FlowDirection Flow = FlowDirection.Left;


    public void InitLimits() {
        this.LimitsX = new Dictionary<Anchor, int> {
            { Anchor.TopLeft, 0 },
            { Anchor.TopCenter, 0 },
            { Anchor.TopRight, 0 },
            { Anchor.CenterLeft, 0 },
            { Anchor.CenterCenter, 0 },
            { Anchor.CenterRight, 0 },
            { Anchor.BottomLeft, 0 },
            { Anchor.BottomCenter, 0 },
            { Anchor.BottomRight, 0 }
        };
        this.LimitsY = new Dictionary<Anchor, int> {
            { Anchor.TopLeft, 0 },
            { Anchor.TopCenter, 0 },
            { Anchor.TopRight, 0 },
            { Anchor.CenterLeft, 0 },
            { Anchor.CenterCenter, 0 },
            { Anchor.CenterRight, 0 },
            { Anchor.BottomLeft, 0 },
            { Anchor.BottomCenter, 0 },
            { Anchor.BottomRight, 0 }
        };
    }


    public void ApplyTo(IFile sourceImage, IFile targetImage, bool isGrayScale, IEnumerable<WatermarkRequest<T>> watermarksLandscape, IEnumerable<WatermarkRequest<T>> watermarksPortrait)
    {
        this.InitLimits();

        using var sourceStream = sourceImage.OpenRead();
        using var bitmap = SKBitmap.Decode(sourceStream);

        var watermarks = watermarksLandscape;

        if (bitmap.Width < bitmap.Height &&
            watermarksPortrait != null &&
            watermarksPortrait.Any())
        {
            watermarks = watermarksPortrait;
        }

        foreach (var watermark in watermarks)
        {
            ApplyTo(bitmap, watermark, isGrayScale);
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

        using var outputStream = File.OpenWrite(targetImage.FullName);
        data.SaveTo(outputStream);
    }


    public abstract SKBitmap ApplyTo(SKBitmap picture, WatermarkRequest<T> request, bool useGrayScale);
}