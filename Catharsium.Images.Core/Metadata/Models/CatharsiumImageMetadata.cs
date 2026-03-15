namespace Catharsium.Images.Core.Metadata.Models;

public class CatharsiumImageMetadata : BaseImageMetadata
{
    public int? Rating { get; internal set; }
    public string? Label { get; internal set; }


    public override bool IsGrayScale =>
        this.Keywords != null && this.Keywords.Any(k => k.Equals("Black and white", StringComparison.InvariantCultureIgnoreCase));

    public override bool IsPrivate =>
        this.Label != null && this.Label.Equals("red", StringComparison.InvariantCultureIgnoreCase);
}