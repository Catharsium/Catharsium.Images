using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;

namespace Catharsium.Images.Watermarking.Interfaces;

public interface IWatermarkingService<T>
{
    void ApplyTo<T>(IFile sourceImage, IFile targetImage, IEnumerable<WatermarkRequest<IFile>> watermarks);
}