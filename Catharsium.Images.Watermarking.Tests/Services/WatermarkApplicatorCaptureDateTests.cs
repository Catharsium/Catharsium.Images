using Catharsium.Images.Core.Metadata.Interfaces;
using Catharsium.Images.Core.Metadata.Models;
using Catharsium.Images.Watermarking._Configuration;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Services;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Files.Interfaces;
using NSubstitute;

namespace Catharsium.Images.Watermarking.Tests.Services;

[TestClass]
public class WatermarkApplicatorCaptureDateTests
{
    [TestMethod]
    public void Apply_PhotoWithoutCaptureDate_UsesLastWriteTimeForFolder() {
        var fileFactory = Substitute.For<IFileFactory>();
        var metadataRetriever = Substitute.For<IMetadataRetriever>();
        var fileNameService = Substitute.For<IFileNameService>();
        fileNameService.SuggestValidFileName(Arg.Any<string>()).ReturnsForAnyArgs(c => c.ArgAt<string>(0));

        var exportFolder = Substitute.For<IDirectory>();
        exportFolder.FullName.Returns("export");
        var sourceFile = Substitute.For<IFile>();
        sourceFile.Name.Returns("photo.jpg");
        sourceFile.Directory.Returns(exportFolder);
        sourceFile.LastWriteTime.Returns(new DateTime(2026, 3, 17, 14, 30, 0));
        fileFactory.CreateFile("photo.jpg").Returns(sourceFile);
        fileFactory.CreateFile(Arg.Is<string>(p => p != "photo.jpg")).Returns(Substitute.For<IFile>());

        var createdFolders = new List<string>();
        fileFactory.CreateDirectory(Arg.Any<string>()).Returns(c => {
            createdFolders.Add(c.Arg<string>());
            var directory = Substitute.For<IDirectory>();
            directory.FullName.Returns(c.Arg<string>());
            return directory;
        });

        metadataRetriever.Get(sourceFile).Returns(new CatharsiumImageMetadata { Name = "photo", Keywords = [] });
        var settings = new WatermarkingSettings {
            DefaultSet = new WatermarkSet { Name = "Default", Watermarks = [] },
            Sets = []
        };
        var target = new WatermarkApplicator(fileFactory, Substitute.For<IWatermarkingService<IFile>>(), metadataRetriever, fileNameService, Substitute.For<IConsole>(), settings);

        target.Apply(["photo.jpg"]);

        Assert.IsTrue(createdFolders[0].StartsWith(Path.Combine("export", "2026-03-17, ")), createdFolders[0]);
    }
}
