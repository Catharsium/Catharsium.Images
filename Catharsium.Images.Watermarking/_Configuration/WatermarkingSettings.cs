namespace Catharsium.Images.Watermarking._Configuration;

public class WatermarkingSettings
{
    /// <summary>
    /// Base folder for watermark images. Relative <see cref="Watermark.Image"/> paths are resolved against it.
    /// </summary>
    public string LogoFolder { get; set; }
    public WatermarkSet DefaultSet { get; set; }
    public WatermarkSet[] Sets { get; set; }


    public string ResolveImagePath(string image) {
        if(string.IsNullOrWhiteSpace(image) || Path.IsPathRooted(image) || string.IsNullOrWhiteSpace(this.LogoFolder)) {
            return image;
        }

        return Path.Combine(this.LogoFolder, image);
    }
}
