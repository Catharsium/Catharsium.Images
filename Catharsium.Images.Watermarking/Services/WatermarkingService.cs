using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

namespace Catharsium.Images.Watermarking.Services;

public abstract class WatermarkingService<T> : IWatermarkingService<T>
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public void ApplyTo<T>(IFile sourceImage, IFile targetImage, IEnumerable<WatermarkRequest<T>> watermarks) {
        Bitmap outputImage;
        using (var image = new Bitmap(sourceImage.OpenRead())) {
            outputImage = (Bitmap)image.Clone();
        }

        foreach (var watermark in watermarks) {
            outputImage = this.ApplyTo(outputImage, watermark, sourceImage.Name.Contains("[bw]"));
        }

        outputImage.Save(targetImage.FullName, ImageFormat.Jpeg);
    }


    public abstract Bitmap ApplyTo<T>(Bitmap picture, WatermarkRequest<T> request, bool useGrayScale);
}