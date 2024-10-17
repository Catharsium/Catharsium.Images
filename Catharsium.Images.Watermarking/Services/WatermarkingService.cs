using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

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


    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public void ApplyTo<T>(IFile sourceImage, IFile targetImage, IEnumerable<WatermarkRequest<T>> watermarks) {
        this.InitLimits();
        Bitmap outputImage;
        using(var image = new Bitmap(sourceImage.OpenRead())) {
            outputImage = (Bitmap)image.Clone();
        }

        foreach(var watermark in watermarks) {
            outputImage = this.ApplyTo(outputImage, watermark, sourceImage.Name.Contains("[bw]"));
        }

        outputImage.Save(targetImage.FullName, ImageFormat.Jpeg);
    }


    public abstract Bitmap ApplyTo<T>(Bitmap picture, WatermarkRequest<T> request, bool useGrayScale);
}