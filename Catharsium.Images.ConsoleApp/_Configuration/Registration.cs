using Catharsium.Images.ConsoleApp._Configuration;
using Catharsium.Images.Watermarking._Configuration;
using Catharsium.Util.Configuration.Extensions;
using Catharsium.Util.IO.Console._Configuration;
using Catharsium.Util.IO.Console.Menu.Interfaces;
using Catharsium.Watermarker.Interfaces.Menu;
using Catharsium.Watermarker.Menu.Apply;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catharsium.Watermarker._Configuration;

public static class Registration
{
    public static IServiceCollection AddWatermarking(this IServiceCollection services, IConfiguration config) {
        var configuration = config.Load<ConsoleAppSettings>();
        return services.AddSingleton<ConsoleAppSettings, ConsoleAppSettings>(provider => configuration)
            .AddConsoleIoUtilities(config)

            .AddImagesWatermarking(config)

            .AddScoped<IMenuActionHandler, WatermarkMenu>()
            .AddScoped<IWatermarkActionHandler, AddAction>();
    }
}