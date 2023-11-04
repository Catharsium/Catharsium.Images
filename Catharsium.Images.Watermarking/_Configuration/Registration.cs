using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Services;
using Catharsium.Util.Configuration.Extensions;
using Catharsium.Util.IO.Files._Configuration;
using Catharsium.Util.IO.Files.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catharsium.Images.Watermarking._Configuration;

public static class Registration
{
    public static IServiceCollection AddImagesWatermarking(this IServiceCollection services, IConfiguration config, string[] files) {
        var configuration = config.Load<WatermarkingSettings>();
        configuration.Files = files;
        return services.AddSingleton<WatermarkingSettings, WatermarkingSettings>(provider => configuration)
            .AddFilesIoUtilities(config)

            .AddScoped<IWatermarkingService<string>, PictureTextWatermarkingService>()
            .AddScoped<IWatermarkingService<IFile>, PictureImageWatermarkingService>();
    }
}