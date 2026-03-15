using Catharsium.Images.Watermarking.Models;

namespace Catharsium.Images.Watermarking._Configuration;

public class Watermark
{
    public required string Image { get; set; }
    public double Scale { get; set; }
    public Anchor Anchor { get; set; }
    public FlowDirection Flow { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double? BackgroundMarginX { get; set; }
    public double? BackgroundMarginY { get; set; }
}