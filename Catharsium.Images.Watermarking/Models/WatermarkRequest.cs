namespace Catharsium.Images.Watermarking.Models;

public class WatermarkRequest<T>
{
    public required T Image { get; set; }
    public double Scale { get; set; }
    public Anchor Anchor { get; set; }
    public FlowDirection Flow { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double? BackgroundMarginX { get; set; }
    public double? BackgroundMarginY { get; set; }



    public override string ToString() {
        return $"{{ Scale: {this.Scale}, " +
            $"Anchor: {this.Anchor}, " +
            $"Flow: {this.Flow}, " +
            $"OffsetX: {this.OffsetX}, " +
            $"OffsetY: {this.OffsetY}, " +
            $"BackgroundMarginX: {this.BackgroundMarginX}, " +
            $"BackgroundMarginY: {this.BackgroundMarginY}}}";
    }
}