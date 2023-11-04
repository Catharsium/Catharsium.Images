namespace Catharsium.Images.Watermarking.Models;

public class WatermarkRequest<T>
{
    public required T Watermark { get; set; }
    public double Scale { get; set; }
    public Anchor Anchor { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
}