
namespace Catharsium.Images.Core.Metadata.Models;

public abstract class BaseImageMetadata
{
    public required string Name { get; set; }
    public string? Sublocation { get; internal set; }
    public string? City { get; internal set; }
    public string? Country { get; internal set; }
    public required string[] Keywords { get; set; }
    public string? Series { get; internal set; }
    public DateTimeOffset? Timestamp { get; internal set; }
    public string? Caption { get; internal set; }


    public abstract bool IsGrayScale { get; }

    public abstract bool IsPrivate { get; }
}