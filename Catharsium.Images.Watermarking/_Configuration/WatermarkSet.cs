namespace Catharsium.Images.Watermarking._Configuration;

public class WatermarkSet
{
    public string Name { get; set; }
    public bool OverrideInputFiles { get; set; }
    public Watermark[] ImageWatermarks { get; set; }
    public Watermark[] TextWatermarks { get; set; }
}