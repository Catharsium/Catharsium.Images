using Catharsium.Images.Watermarking.Models;

namespace Catharsium.Watermarker._Configuration;

public class Watermark
{
    public required string Source { get; set; }
    public double Scale { get; set; }
    public Anchor Anchor { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
}