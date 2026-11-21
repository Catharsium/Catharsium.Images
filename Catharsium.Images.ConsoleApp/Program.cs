using Catharsium.Images.ConsoleApp._Configuration;
using Catharsium.Images.ConsoleApp.Interfaces;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Console.Menu.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catharsium.Images.ConsoleApp;

public class Program
{
    /// <param name="args">Image files and/or folders to watermark. Without arguments the menu is shown.</param>
    public static async Task Main(string[] args) {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.local.json", optional: true);
        var configuration = builder.Build();

        var serviceProvider = new ServiceCollection()
            .AddWatermarking(configuration)
            .BuildServiceProvider();

        var console = serviceProvider.GetService<IConsole>();
        if(args.Length != 0) {
            var files = serviceProvider.GetService<IInputFileResolver>().Resolve(args);
            if(files.Length != 0) {
                serviceProvider.GetService<IWatermarkApplicator>().Apply(files);
            }
            else {
                console.WriteLine("No images found in the given paths.");
            }
        }
        else {
            var mainMenuActionHandler = serviceProvider.GetService<IMainMenuActionHandler>();
            await mainMenuActionHandler.Run();
        }

        console.ReadLine();
    }
}
