using Catharsium.Images.Core.Metadata.Interfaces;
using Catharsium.Images.Core.Metadata.Models;
using Catharsium.Util.IO.Files.Interfaces;
using MetadataExtractor;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Xmp;

namespace Catharsium.Images.Core.Metadata;

public class CatharsiumMetadataRetriever : IMetadataRetriever
{
    public CatharsiumImageMetadata Get(IFile file) {
        var directories = ImageMetadataReader.ReadMetadata(file.FullName);
        var iptcDirectory = directories.OfType<IptcDirectory>().Single();
        var sublocation = iptcDirectory?.GetDescription(IptcDirectory.TagSubLocation);
        var city = iptcDirectory?.GetDescription(IptcDirectory.TagCity);
        var country = iptcDirectory?.GetDescription(IptcDirectory.TagCountryOrPrimaryLocationName);
        var caption = iptcDirectory?.GetDescription(IptcDirectory.TagCaption);
        var keywords = iptcDirectory?.GetDescription(IptcDirectory.TagKeywords)?.Split(';');
        var series = iptcDirectory?.GetDescription(IptcDirectory.TagObjectName);
        var timestamp = iptcDirectory?.GetDateCreated();

        var xmpDirectory = directories.OfType<XmpDirectory>().FirstOrDefault();
        var rating = xmpDirectory?.XmpMeta?.GetPropertyInteger(XmpMetadata.AdobeNamespace, XmpMetadata.XmpRating);
        var label = xmpDirectory?.XmpMeta?.GetPropertyString(XmpMetadata.AdobeNamespace, XmpMetadata.XmpLabel)?.ToLower();

        return new CatharsiumImageMetadata {
            Name = file.Name,
            Caption = caption,
            Series = series,
            Sublocation = sublocation,
            City = city,
            Country = country,
            Timestamp = timestamp,
            Rating = rating,
            Label = label,
            Keywords = keywords!
        };
    }
}