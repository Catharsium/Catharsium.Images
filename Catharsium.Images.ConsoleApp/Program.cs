using Catharsium.Images.ConsoleApp._Configuration;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Console.Menu.Interfaces;
using Catharsium.Util.IO.Files.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catharsium.Images.ConsoleApp;

public class Program
{
    public static async Task Main(string[] args) {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile($"appsettings.json");
        var configuration = builder.Build();

        var serviceProvider = new ServiceCollection()
            .AddWatermarking(configuration)
            .BuildServiceProvider();

        var files = args;
        if(files.Length == 0) {
            var folder = serviceProvider.GetService<IFileFactory>().CreateDirectory("D:\\Onedrive\\Portfolio\\_Export\\Test");
            files = [.. folder.GetFiles().Select(f => f.FullName)];
        }

        if(files.Length != 0) {
            var actionHandler = serviceProvider.GetService<IWatermarkApplicator>();
            actionHandler.Apply(files);
        }
        else {
            var mainMenuActionHandler = serviceProvider.GetService<IMainMenuActionHandler>();
            await mainMenuActionHandler.Run();
        }

        serviceProvider.GetService<IConsole>().ReadLine();
    }
}