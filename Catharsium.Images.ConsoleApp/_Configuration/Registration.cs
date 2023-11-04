using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Services;
using Catharsium.Util.Configuration.Extensions;
using Catharsium.Util.IO.Console._Configuration;
using Catharsium.Util.IO.Console.Menu.Interfaces;
using Catharsium.Util.IO.Files._Configuration;
using Catharsium.Util.IO.Files.Interfaces;
using Catharsium.Watermarker.Interfaces.Menu;
using Catharsium.Watermarker.Menu.Apply;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catharsium.Watermarker._Configuration;

public static class Registration
{
    public static IServiceCollection AddWatermarking(this IServiceCollection services, IConfiguration config, string[] files) {
        var configuration = config.Load<ImagesSettings>();
        configuration.Files = files;
        return services.AddSingleton<ImagesSettings, ImagesSettings>(provider => configuration)
            .AddConsoleIoUtilities(config)
            .AddFilesIoUtilities(config)

            .AddScoped<IWatermarkingService<string>, PictureTextWatermarkingService>()
            .AddScoped<IWatermarkingService<IFile>, PictureImageWatermarkingService>()

            .AddScoped<IMenuActionHandler, ApplyMenu>()
            .AddScoped<IApplyActionHandler, AddAction>();
    }
}