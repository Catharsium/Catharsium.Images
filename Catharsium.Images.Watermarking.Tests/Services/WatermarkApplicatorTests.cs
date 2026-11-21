using Catharsium.Images.Core.Metadata.Interfaces;
using Catharsium.Images.Core.Metadata.Models;
using Catharsium.Images.Watermarking._Configuration;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Images.Watermarking.Services;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Files.Interfaces;
using NSubstitute;

namespace Catharsium.Images.Watermarking.Tests.Services;

[TestClass]
public class WatermarkApplicatorTests
{
    private static readonly string SourceFolder = Path.Combine(Path.GetTempPath(), "Export");
    private static readonly string MainFolder = Path.Combine(SourceFolder, "2026-03-15, NK Pool (Sporthal De Schelp)");

    private IFileFactory fileFactory = null!;
    private IWatermarkingService<IFile> watermarkingService = null!;
    private IMetadataRetriever metadataRetriever = null!;
    private IFileNameService fileNameService = null!;
    private IConsole console = null!;
    private WatermarkingSettings settings = null!;

    private Dictionary<string, IFile> files = null!;
    private Dictionary<string, IDirectory> directories = null!;
    private Dictionary<string, CatharsiumImageMetadata> metadata = null!;
    private bool directoriesExist;

    private WatermarkApplicator target = null!;


    [TestInitialize]
    public void Initialize() {
        this.files = [];
        this.directories = [];
        this.metadata = [];
        this.directoriesExist = false;

        this.fileFactory = Substitute.For<IFileFactory>();
        this.fileFactory.CreateFile(Arg.Any<string>()).Returns(c => this.GetFile(c.Arg<string>()));
        this.fileFactory.CreateDirectory(Arg.Any<string>()).Returns(c => this.GetDirectory(c.Arg<string>()));

        this.metadataRetriever = Substitute.For<IMetadataRetriever>();
        this.metadataRetriever.Get(Arg.Any<IFile>()).Returns(c => this.metadata[c.Arg<IFile>().FullName]);

        this.fileNameService = Substitute.For<IFileNameService>();
        this.fileNameService.SuggestValidFileName(Arg.Any<string>(), Arg.Any<string>()).Returns(c => c.ArgAt<string>(0));

        this.watermarkingService = Substitute.For<IWatermarkingService<IFile>>();
        this.console = Substitute.For<IConsole>();

        this.settings = new WatermarkingSettings {
            DefaultSet = CreateSet(null, "Default.png"),
            Sets = [
                CreateSet("NK Pool 2026", "NK Pool.png", "Default.png"),
                CreateSet("Club", "Club.png")
            ]
        };

        this.target = new WatermarkApplicator(this.fileFactory, this.watermarkingService, this.metadataRetriever, this.fileNameService, this.console, this.settings);
    }


    #region Helpers

    private static WatermarkSet CreateSet(string? name, params string[] images) {
        return new WatermarkSet {
            Name = name,
            Watermarks = images.Select(i => new Watermark { Image = i, Scale = 0.13, Anchor = Anchor.BottomRight }).ToList()
        };
    }


    private IFile GetFile(string path) {
        if(!this.files.TryGetValue(path, out var file)) {
            var directory = this.GetDirectory(Path.GetDirectoryName(path)!);
            file = Substitute.For<IFile>();
            file.FullName.Returns(path);
            file.Name.Returns(Path.GetFileName(path));
            file.Directory.Returns(directory);
            this.files[path] = file;
        }

        return file;
    }


    private IDirectory GetDirectory(string path) {
        if(!this.directories.TryGetValue(path, out var directory)) {
            directory = Substitute.For<IDirectory>();
            directory.FullName.Returns(path);
            directory.Name.Returns(Path.GetFileName(path));
            directory.Exists.Returns(this.directoriesExist);
            this.directories[path] = directory;
        }

        return directory;
    }


    private string AddSourceFile(string name, bool isPrivate = false, bool isGrayScale = false, params string[] keywords) {
        var path = Path.Combine(SourceFolder, name);
        var allKeywords = isGrayScale ? keywords.Append("Black and white").ToArray() : keywords;
        this.metadata[path] = new CatharsiumImageMetadata {
            Name = name,
            Keywords = allKeywords,
            Series = "NK Pool",
            Sublocation = "Sporthal De Schelp",
            Timestamp = new DateTimeOffset(2026, 3, 15, 14, 30, 0, TimeSpan.FromHours(1)),
            Label = isPrivate ? "red" : null
        };
        return path;
    }


    private static string Folder(params string[] parts) => Path.Combine([MainFolder, .. parts]);

    #endregion

    #region GetWatermarkSet

    [TestMethod]
    public void GetWatermarkSet_NullKeywords_ReturnsDefaultSet() {
        var actual = this.target.GetWatermarkSet(null!);
        Assert.AreSame(this.settings.DefaultSet, actual);
    }


    [TestMethod]
    public void GetWatermarkSet_NoKeywords_ReturnsDefaultSet() {
        var actual = this.target.GetWatermarkSet([]);
        Assert.AreSame(this.settings.DefaultSet, actual);
    }


    [TestMethod]
    public void GetWatermarkSet_NoMatchingKeyword_ReturnsDefaultSet() {
        var actual = this.target.GetWatermarkSet(["Sport", "Black and white"]);
        Assert.AreSame(this.settings.DefaultSet, actual);
    }


    [TestMethod]
    [DataRow("NK Pool 2026", 0)]
    [DataRow("Club", 1)]
    public void GetWatermarkSet_MatchingKeyword_ReturnsThatSet(string keyword, int expectedIndex) {
        var actual = this.target.GetWatermarkSet(["Sport", keyword, "Black and white"]);
        Assert.AreSame(this.settings.Sets[expectedIndex], actual);
    }


    [TestMethod]
    public void GetWatermarkSet_MultipleMatchingKeywords_ReturnsSetOfFirstKeyword() {
        var actual = this.target.GetWatermarkSet(["Club", "NK Pool 2026"]);
        Assert.AreSame(this.settings.Sets[1], actual);
    }


    [TestMethod]
    public void GetWatermarkSet_NoSetsConfigured_ReturnsDefaultSet() {
        this.settings.Sets = [];
        var actual = this.target.GetWatermarkSet(["NK Pool 2026"]);
        Assert.AreSame(this.settings.DefaultSet, actual);
    }

    #endregion

    #region Apply: folder structure

    [TestMethod]
    public void Apply_CreatesMainFolderNamedAfterDateSeriesAndSublocation() {
        var file = this.AddSourceFile("IMG_0001.jpg");

        this.target.Apply([file]);

        this.fileNameService.Received().SuggestValidFileName("NK Pool (Sporthal De Schelp)", Arg.Any<string>());
        this.directories[MainFolder].Received(1).Create();
    }


    [TestMethod]
    public void Apply_CreatesPublicAndPrivateSubfolders() {
        var file = this.AddSourceFile("IMG_0001.jpg");

        this.target.Apply([file]);

        string[] expected = [
            Folder("Public"),
            Folder("Public", "Color"),
            Folder("Public", "Color [Logo]"),
            Folder("Public", "B&W"),
            Folder("Public", "B&W [Logo]"),
            Folder("Private"),
            Folder("Private", "Color"),
            Folder("Private", "Color [Logo]"),
            Folder("Private", "B&W"),
            Folder("Private", "B&W [Logo]"),
        ];
        foreach(var folder in expected) {
            Assert.IsTrue(this.directories.ContainsKey(folder), $"Folder not requested: {folder}");
            this.directories[folder].Received(1).Create();
        }
    }


    [TestMethod]
    public void Apply_FoldersAlreadyExist_DoesNotCreateThemAgain() {
        var file = this.AddSourceFile("IMG_0001.jpg");
        this.directoriesExist = true;

        this.target.Apply([file]);

        foreach(var directory in this.directories.Values) {
            directory.DidNotReceive().Create();
        }
    }


    [TestMethod]
    public void Apply_MultipleFiles_CreatesFoldersOnlyOnce() {
        var file1 = this.AddSourceFile("IMG_0001.jpg");
        var file2 = this.AddSourceFile("IMG_0002.jpg", isPrivate: true);

        this.target.Apply([file1, file2]);

        this.directories[MainFolder].Received(1).Create();
        this.directories[Folder("Public", "Color")].Received(1).Create();
        this.fileNameService.Received(1).SuggestValidFileName(Arg.Any<string>(), Arg.Any<string>());
    }


    [TestMethod]
    [DataRow(false, false, "Public", "Color")]
    [DataRow(false, true, "Public", "B&W")]
    [DataRow(true, false, "Private", "Color")]
    [DataRow(true, true, "Private", "B&W")]
    public void Apply_WritesWatermarkedImageToLogoFolderAndMovesSource(bool isPrivate, bool isGrayScale, string visibilityFolder, string colorFolder) {
        var path = this.AddSourceFile("IMG_0001.jpg", isPrivate, isGrayScale);
        var expectedOutput = Path.Combine(Folder(visibilityFolder, $"{colorFolder} [Logo]"), "IMG_0001.jpg");
        var expectedMove = Path.Combine(Folder(visibilityFolder, colorFolder), "IMG_0001.jpg");

        this.target.Apply([path]);

        this.watermarkingService.Received(1).ApplyTo(
            this.files[path],
            Arg.Is<IFile>(f => f.FullName == expectedOutput),
            isGrayScale,
            Arg.Any<IEnumerable<WatermarkRequest<IFile>>>(),
            Arg.Any<IEnumerable<WatermarkRequest<IFile>>>());
        this.files[path].Received(1).MoveTo(expectedMove);
    }

    #endregion

    #region Apply: watermarks

    [TestMethod]
    public void Apply_WithoutMatchingKeyword_UsesDefaultSetWatermarks() {
        var path = this.AddSourceFile("IMG_0001.jpg", keywords: "Sport");
        IEnumerable<WatermarkRequest<IFile>>? actual = null;
        this.watermarkingService.ApplyTo(Arg.Any<IFile>(), Arg.Any<IFile>(), Arg.Any<bool>(), Arg.Do<IEnumerable<WatermarkRequest<IFile>>>(w => actual = w.ToList()), Arg.Any<IEnumerable<WatermarkRequest<IFile>>>());

        this.target.Apply([path]);

        Assert.IsNotNull(actual);
        CollectionAssert.AreEqual(new[] { "Default.png" }, actual.Select(w => w.Image.FullName).ToArray());
    }


    [TestMethod]
    public void Apply_WithSetKeyword_UsesWatermarksOfThatSet() {
        var path = this.AddSourceFile("IMG_0001.jpg", keywords: "NK Pool 2026");
        IEnumerable<WatermarkRequest<IFile>>? actual = null;
        this.watermarkingService.ApplyTo(Arg.Any<IFile>(), Arg.Any<IFile>(), Arg.Any<bool>(), Arg.Do<IEnumerable<WatermarkRequest<IFile>>>(w => actual = w.ToList()), Arg.Any<IEnumerable<WatermarkRequest<IFile>>>());

        this.target.Apply([path]);

        Assert.IsNotNull(actual);
        CollectionAssert.AreEqual(new[] { "NK Pool.png", "Default.png" }, actual.Select(w => w.Image.FullName).ToArray());
    }


    [TestMethod]
    public void Apply_MapsWatermarkSettingsToRequest() {
        var watermark = new Watermark {
            Image = "Logo.png",
            Scale = 0.2,
            Anchor = Anchor.TopLeft,
            Flow = FlowDirection.Left,
            OffsetX = 0.03,
            OffsetY = -0.025,
            BackgroundMarginX = 0.1,
            BackgroundMarginY = 0.2
        };
        this.settings.DefaultSet = new WatermarkSet { Watermarks = [watermark] };
        var path = this.AddSourceFile("IMG_0001.jpg");
        List<WatermarkRequest<IFile>>? actual = null;
        this.watermarkingService.ApplyTo(Arg.Any<IFile>(), Arg.Any<IFile>(), Arg.Any<bool>(), Arg.Do<IEnumerable<WatermarkRequest<IFile>>>(w => actual = w.ToList()), Arg.Any<IEnumerable<WatermarkRequest<IFile>>>());

        this.target.Apply([path]);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1, actual.Count);
        var request = actual[0];
        Assert.AreEqual("Logo.png", request.Image.FullName);
        Assert.AreEqual(watermark.Scale, request.Scale);
        Assert.AreEqual(watermark.Anchor, request.Anchor);
        Assert.AreEqual(watermark.Flow, request.Flow);
        Assert.AreEqual(watermark.OffsetX, request.OffsetX);
        Assert.AreEqual(watermark.OffsetY, request.OffsetY);
        Assert.AreEqual(watermark.BackgroundMarginX, request.BackgroundMarginX);
        Assert.AreEqual(watermark.BackgroundMarginY, request.BackgroundMarginY);
    }


    [TestMethod]
    public void Apply_SetWithoutPortraitWatermarks_PassesNullForPortrait() {
        var path = this.AddSourceFile("IMG_0001.jpg");

        this.target.Apply([path]);

        this.watermarkingService.Received(1).ApplyTo(Arg.Any<IFile>(), Arg.Any<IFile>(), Arg.Any<bool>(), Arg.Any<IEnumerable<WatermarkRequest<IFile>>>(), Arg.Is<IEnumerable<WatermarkRequest<IFile>>>(w => w == null));
    }


    [TestMethod]
    public void Apply_SetWithPortraitWatermarks_PassesPortraitWatermarks() {
        this.settings.Sets[0].WatermarksPortrait = [new Watermark { Image = "Portrait.png", Scale = 0.2, Anchor = Anchor.BottomCenter }];
        var path = this.AddSourceFile("IMG_0001.jpg", keywords: "NK Pool 2026");
        List<WatermarkRequest<IFile>>? actual = null;
        this.watermarkingService.ApplyTo(Arg.Any<IFile>(), Arg.Any<IFile>(), Arg.Any<bool>(), Arg.Any<IEnumerable<WatermarkRequest<IFile>>>(), Arg.Do<IEnumerable<WatermarkRequest<IFile>>>(w => actual = w?.ToList()));

        this.target.Apply([path]);

        Assert.IsNotNull(actual);
        CollectionAssert.AreEqual(new[] { "Portrait.png" }, actual.Select(w => w.Image.FullName).ToArray());
    }

    #endregion
}
