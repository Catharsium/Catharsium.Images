namespace Catharsium.Images.Watermarking._Configuration;

public class WatermarkingSettings
{
    public bool OverrideInputFiles { get; set; }
    public Watermark[] ImageWatermarks { get; set; }
    public Watermark[] TextWatermarks { get; set; }
}