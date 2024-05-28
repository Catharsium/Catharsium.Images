using Catharsium.Images.ConsoleApp.Interfaces.Menu;
using MetadataExtractor;

namespace Catharsium.Images.ConsoleApp.Menu.Metadata;

internal class ReadMetadataAction : IMetadataActionHandler
{
    public string MenuName { get; }

    public async Task Run() {
        IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata("D:\\OneDrive\\2024-04-16 09-31-41, Pantropica (Netherlands - Luttelgeest - Pantropica), , [Green] [4].jpg");

        foreach (var directory in directories) {
            foreach (var tag in directory.Tags) {
                Console.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");
            }
        }
    }
}