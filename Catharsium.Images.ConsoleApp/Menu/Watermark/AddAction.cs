using Catharsium.Images.ConsoleApp._Configuration;
using Catharsium.Images.ConsoleApp.Interfaces;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Watermarker.Interfaces.Menu;

namespace Catharsium.Images.ConsoleApp.Menu.Watermark;

public class AddAction(IWatermarkApplicator watermarkApplicator, IInputFileResolver inputFileResolver, IConsole console, ConsoleAppSettings settings)
    : IWatermarkActionHandler
{
    public string MenuName => "Add";


    public Task Run() {
        var defaultFolder = settings.InputFolder;
        var prompt = string.IsNullOrWhiteSpace(defaultFolder)
            ? "Folder with images: "
            : $"Folder with images (empty for {defaultFolder}): ";
        var input = console.AskForText(prompt);
        var folder = string.IsNullOrWhiteSpace(input) ? defaultFolder : input;
        if(string.IsNullOrWhiteSpace(folder)) {
            console.WriteLine("No folder given and no InputFolder configured.");
            return Task.CompletedTask;
        }

        var files = inputFileResolver.Resolve([folder]);
        if(files.Length == 0) {
            console.WriteLine($"No images found in {folder}");
            return Task.CompletedTask;
        }

        watermarkApplicator.Apply(files);
        return Task.CompletedTask;
    }
}
