using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;
using System.Drawing;

namespace Catharsium.Images.Watermarking.Interfaces;

public interface IWatermarkingService<T>
{
    void ApplyTo<T>(IFile sourceImage, IFile targetImage, IEnumerable<WatermarkRequest<T>> watermarks);
    Bitmap ApplyTo<T>(Bitmap picture, WatermarkRequest<T> request, bool useGrayScale);
}