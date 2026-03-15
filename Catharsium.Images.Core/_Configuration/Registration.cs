using Catharsium.Images.Core.Metadata;
using Catharsium.Images.Core.Metadata.Interfaces;
using Catharsium.Util.Configuration.Extensions;
using Catharsium.Util.IO.Files._Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catharsium.Images.Core._Configuration;

public static class Registration
{
    public static IServiceCollection AddImagesCore(this IServiceCollection services, IConfiguration config) {
        var configuration = config.Load<CoreSettings>();
        return services.AddSingleton<CoreSettings, CoreSettings>(provider => configuration)
            .AddFilesIoUtilities(config)

            .AddScoped<IMetadataRetriever, CatharsiumMetadataRetriever>();
    }
}