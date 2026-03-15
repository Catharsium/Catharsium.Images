using Catharsium.Images.Core._Configuration;
using Catharsium.Images.Core.Metadata.Interfaces;
using Catharsium.Util.IO.Files.Interfaces;
using Catharsium.Util.Testing.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Catharsium.Images.Core.Tests._Configuration;

[TestClass]
public class RegistrationTests
{
    [TestMethod]
    public void AddImagesCore_RegistersDependencies() {
        var serviceCollection = Substitute.For<IServiceCollection>();
        var configuration = Substitute.For<IConfiguration>();

        serviceCollection.AddImagesCore(configuration);
        serviceCollection.ReceivedRegistration<IMetadataRetriever>();
    }


    [TestMethod]
    public void AddImagesCore_RegistersPackages() {
        var serviceCollection = Substitute.For<IServiceCollection>();
        var configuration = Substitute.For<IConfiguration>();

        serviceCollection.AddImagesCore(configuration);
        serviceCollection.ReceivedRegistration<IFileFactory>();
    }
}