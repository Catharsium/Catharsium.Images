using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;
using SkiaSharp;

namespace Catharsium.Images.Watermarking.Interfaces;

public interface IWatermarkingService<T>
{
    void ApplyTo<T>(IFile sourceImage, IFile targetImage, bool isGrayScale, IEnumerable<WatermarkRequest<T>> watermarksLandscap, IEnumerable<WatermarkRequest<T>> watermarksPortrait);
    SKBitmap ApplyTo<T>(SKBitmap picture, WatermarkRequest<T> request, bool useGrayScale);
}