using Catharsium.Images.Core.Metadata.Interfaces;
using Catharsium.Images.Core.Metadata.Models;
using Catharsium.Util.IO.Files.Interfaces;
using MetadataExtractor;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Xmp;

namespace Catharsium.Images.Core.Metadata;

public class LightroomMetadataRetriever : IMetadataRetriever
{
    public CatharsiumImageMetadata Get(IFile file) {
        var result = new CatharsiumImageMetadata {
            Name = file.Name,
            Keywords = []
        };

        var directories = ImageMetadataReader.ReadMetadata(file.FullName);
        var iptcDirectories = directories.OfType<IptcDirectory>();

        if(!iptcDirectories.Any()) {
            return result;
        }

        var iptcDirectory = iptcDirectories.First();
        result.Sublocation = iptcDirectory?.GetDescription(IptcDirectory.TagSubLocation);
        result.City = iptcDirectory?.GetDescription(IptcDirectory.TagCity);
        result.Country = iptcDirectory?.GetDescription(IptcDirectory.TagCountryOrPrimaryLocationName);
        result.Caption = iptcDirectory?.GetDescription(IptcDirectory.TagCaption);
        result.Series = iptcDirectory?.GetDescription(IptcDirectory.TagObjectName);
        result.Timestamp = iptcDirectory?.GetDateCreated();

        var keywords = iptcDirectory?.GetDescription(IptcDirectory.TagKeywords);
        if(keywords != null) {
            result.Keywords = keywords.Split(';');
        }

        var xmpDirectories = directories.OfType<XmpDirectory>();
        if(!xmpDirectories.Any()) {
            return result;
        }

        var xmpMeta = xmpDirectories.First().XmpMeta;
        if(xmpMeta == null) {
            return result;
        }

        // GetPropertyInteger throws when the property is missing, e.g. for an unrated photo.
        if(xmpMeta.DoesPropertyExist(XmpMetadata.AdobeNamespace, XmpMetadata.XmpRating)) {
            result.Rating = xmpMeta.GetPropertyInteger(XmpMetadata.AdobeNamespace, XmpMetadata.XmpRating);
        }

        result.Label = xmpMeta.GetPropertyString(XmpMetadata.AdobeNamespace, XmpMetadata.XmpLabel)?.ToLower();

        return result;
    }
}