using Catharsium.Images.ConsoleApp.Interfaces;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Files.Interfaces;

namespace Catharsium.Images.ConsoleApp.Services;

public class InputFileResolver(IFileFactory fileFactory, IConsole console)
    : IInputFileResolver
{
    public string[] Resolve(IEnumerable<string> paths) {
        var result = new List<string>();
        foreach(var path in paths ?? []) {
            if(string.IsNullOrWhiteSpace(path)) {
                continue;
            }

            var cleanPath = path.Trim().Trim('"');
            var directory = fileFactory.CreateDirectory(cleanPath);
            if(directory.Exists) {
                result.AddRange(directory.GetFiles().Select(f => f.FullName).OrderBy(f => f));
                continue;
            }

            var file = fileFactory.CreateFile(cleanPath);
            if(file.Exists) {
                result.Add(file.FullName);
                continue;
            }

            console.WriteLine($"Not found: {cleanPath}");
        }

        return [.. result];
    }
}
