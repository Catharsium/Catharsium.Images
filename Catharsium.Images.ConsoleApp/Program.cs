using Catharsium.Images.Watermarking._Configuration;
using Catharsium.Util.IO.Console.Menu.Interfaces;
using Catharsium.Watermarker._Configuration;
using Catharsium.Watermarker.Interfaces.Menu;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catharsium.Watermarker;

public class Program
{
    public static async Task Main(string[] args) {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile($"appsettings.json");
        var configuration = builder.Build();

        var serviceProvider = new ServiceCollection()
            .AddWatermarking(configuration, args)
            .BuildServiceProvider();

        var actionHandler = serviceProvider.GetService<IApplyActionHandler>();
        await actionHandler.Run();
        //var mainMenuActionHandler = serviceProvider.GetService<IMainMenuActionHandler>();
        //await mainMenuActionHandler.Run();
    }
}