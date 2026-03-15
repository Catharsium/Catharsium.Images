using Catharsium.Images.Core.Metadata.Models;
using Catharsium.Util.IO.Files.Interfaces;

namespace Catharsium.Images.Core.Metadata.Interfaces;

public interface IMetadataRetriever
{
    CatharsiumImageMetadata Get(IFile file);
}