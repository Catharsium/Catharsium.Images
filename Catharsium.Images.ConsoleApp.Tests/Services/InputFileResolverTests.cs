using Catharsium.Images.ConsoleApp.Services;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Files.Interfaces;
using NSubstitute;

namespace Catharsium.Images.ConsoleApp.Tests.Services;

[TestClass]
public class InputFileResolverTests
{
    private IFileFactory fileFactory = null!;
    private IConsole console = null!;
    private InputFileResolver target = null!;


    [TestInitialize]
    public void Setup() {
        this.fileFactory = Substitute.For<IFileFactory>();
        this.console = Substitute.For<IConsole>();
        this.target = new InputFileResolver(this.fileFactory, this.console);
    }


    private void SetupDirectory(string path, params string[] files) {
        var directoryFiles = files.Select(this.CreateFile).ToArray();
        var directory = Substitute.For<IDirectory>();
        directory.Exists.Returns(true);
        directory.GetFiles().Returns(directoryFiles);
        this.fileFactory.CreateDirectory(path).Returns(directory);
    }


    private IFile CreateFile(string path) {
        var file = Substitute.For<IFile>();
        file.Exists.Returns(true);
        file.FullName.Returns(path);
        return file;
    }


    [TestMethod]
    public void Resolve_Folder_ReturnsFilesInFolder() {
        this.SetupDirectory("folder", "b.jpg", "a.jpg");

        var actual = this.target.Resolve(["folder"]);
        CollectionAssert.AreEqual(new[] { "a.jpg", "b.jpg" }, actual);
    }


    [TestMethod]
    public void Resolve_QuotedFolder_StripsQuotes() {
        this.SetupDirectory("my folder", "a.jpg");

        var actual = this.target.Resolve(["\"my folder\" "]);
        CollectionAssert.AreEqual(new[] { "a.jpg" }, actual);
    }


    [TestMethod]
    public void Resolve_File_ReturnsFile() {
        this.fileFactory.CreateDirectory("a.jpg").Returns(Substitute.For<IDirectory>());
        var file = this.CreateFile("full/a.jpg");
        this.fileFactory.CreateFile("a.jpg").Returns(file);

        var actual = this.target.Resolve(["a.jpg"]);
        CollectionAssert.AreEqual(new[] { "full/a.jpg" }, actual);
    }


    [TestMethod]
    public void Resolve_MissingPath_ReturnsNothingAndReports() {
        this.fileFactory.CreateDirectory("missing").Returns(Substitute.For<IDirectory>());
        this.fileFactory.CreateFile("missing").Returns(Substitute.For<IFile>());

        var actual = this.target.Resolve(["missing"]);
        Assert.AreEqual(0, actual.Length);
        this.console.Received().WriteLine(Arg.Is<string>(s => s.Contains("missing")));
    }
}
