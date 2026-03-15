namespace Catharsium.Images.Watermarking._Configuration;

public class WatermarkSet
{
    public string Name { get; set; }
    public List<Watermark> Watermarks { get; set; }
    public List<Watermark> WatermarksPortrait { get; set; }
}