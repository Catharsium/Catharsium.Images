using Catharsium.Images.Core.Metadata.Interfaces;
using Catharsium.Images.Watermarking._Configuration;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Util.IO.Files.Interfaces;
using Catharsium.Util.Testing.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Catharsium.Images.Watermarking.Tests._Configuration;

[TestClass]
public class RegistrationTests
{
    [TestMethod]
    public void AddImagesWatermarking_RegistersDependencies() {
        var serviceCollection = Substitute.For<IServiceCollection>();
        var configuration = Substitute.For<IConfiguration>();

        serviceCollection.AddImagesWatermarking(configuration);
        serviceCollection.ReceivedRegistration<IWatermarkApplicator>();
        serviceCollection.ReceivedRegistration<IWatermarkingService<string>>();
        serviceCollection.ReceivedRegistration<IWatermarkingService<IFile>>();
    }


    [TestMethod]
    public void AddImagesWatermarking_RegistersProjects() {
        var serviceCollection = Substitute.For<IServiceCollection>();
        var configuration = Substitute.For<IConfiguration>();

        serviceCollection.AddImagesWatermarking(configuration);
        serviceCollection.ReceivedRegistration<IMetadataRetriever>();
    }


    [TestMethod]
    public void AddImagesWatermarking_RegistersPackages() {
        var serviceCollection = Substitute.For<IServiceCollection>();
        var configuration = Substitute.For<IConfiguration>();

        serviceCollection.AddImagesWatermarking(configuration);
        serviceCollection.ReceivedRegistration<IFileFactory>();
    }
}