namespace Catharsium.Images.Watermarking._Configuration;

public class WatermarkingSettings
{
    public bool OverrideInputFiles { get; set; }
    public string[] Files { get; set; }
    public Watermark[] ImageWatermarks { get; set; }
}