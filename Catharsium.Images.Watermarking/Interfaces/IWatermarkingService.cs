using Catharsium.Images.Watermarking.Models;
using System.Drawing;

namespace Catharsium.Images.Watermarking.Interfaces;

public interface IWatermarkingService<T>
{
    Bitmap ApplyTo(Bitmap picture, WatermarkRequest<T> request);
}