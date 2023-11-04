namespace Catharsium.Watermarker._Configuration;

public class ImagesSettings
{
    public bool OverrideInputFiles { get; set; }
    public string[] Files { get; set; }
    public Watermark[] ImageWatermarks { get; set; }
}