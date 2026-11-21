using Catharsium.Images.Core.Metadata;
using Catharsium.Util.IO.Files.Interfaces;
using NSubstitute;

namespace Catharsium.Images.Core.Tests.Metadata;

[TestClass]
public class LightroomMetadataRetrieverTests
{
    private readonly List<string> tempFiles = [];
    private LightroomMetadataRetriever target = null!;


    [TestInitialize]
    public void Initialize() {
        this.target = new LightroomMetadataRetriever();
    }


    [TestCleanup]
    public void Cleanup() {
        foreach(var path in this.tempFiles.Where(File.Exists)) {
            File.Delete(path);
        }
    }


    private IFile CreateFile(TestJpegBuilder builder, string name = "IMG_0001.jpg") {
        var path = builder.WriteToTempFile();
        this.tempFiles.Add(path);

        var file = Substitute.For<IFile>();
        file.FullName.Returns(path);
        file.Name.Returns(name);
        return file;
    }


    #region Without metadata

    [TestMethod]
    public void Get_FileWithoutMetadata_ReturnsOnlyNameAndEmptyKeywords() {
        var file = this.CreateFile(new TestJpegBuilder(), "Empty.jpg");

        var actual = this.target.Get(file);

        Assert.AreEqual("Empty.jpg", actual.Name);
        Assert.IsNotNull(actual.Keywords);
        Assert.AreEqual(0, actual.Keywords.Length);
        Assert.IsNull(actual.Series);
        Assert.IsNull(actual.Sublocation);
        Assert.IsNull(actual.City);
        Assert.IsNull(actual.Country);
        Assert.IsNull(actual.Caption);
        Assert.IsNull(actual.Timestamp);
        Assert.IsNull(actual.Rating);
        Assert.IsNull(actual.Label);
        Assert.IsFalse(actual.IsGrayScale);
        Assert.IsFalse(actual.IsPrivate);
    }

    #endregion

    #region IPTC

    [TestMethod]
    public void Get_WithIptc_ReadsLocationAndDescriptionFields() {
        var file = this.CreateFile(new TestJpegBuilder()
            .WithObjectName("NK Pool")
            .WithSublocation("Sporthal De Schelp")
            .WithCity("Zoetermeer")
            .WithCountry("Nederland")
            .WithCaption("Finale 9-ball"));

        var actual = this.target.Get(file);

        Assert.AreEqual("NK Pool", actual.Series);
        Assert.AreEqual("Sporthal De Schelp", actual.Sublocation);
        Assert.AreEqual("Zoetermeer", actual.City);
        Assert.AreEqual("Nederland", actual.Country);
        Assert.AreEqual("Finale 9-ball", actual.Caption);
    }


    [TestMethod]
    public void Get_WithIptcKeywords_ReturnsEachKeywordSeparately() {
        var file = this.CreateFile(new TestJpegBuilder()
            .WithKeyword("NK Pool 2026")
            .WithKeyword("Black and white")
            .WithKeyword("Sport"));

        var actual = this.target.Get(file);

        CollectionAssert.AreEqual(new[] { "NK Pool 2026", "Black and white", "Sport" }, actual.Keywords);
    }


    [TestMethod]
    public void Get_WithIptcButNoKeywords_ReturnsEmptyKeywords() {
        var file = this.CreateFile(new TestJpegBuilder().WithObjectName("NK Pool"));

        var actual = this.target.Get(file);

        Assert.IsNotNull(actual.Keywords);
        Assert.AreEqual(0, actual.Keywords.Length);
    }


    [TestMethod]
    [DataRow("Black and white", true)]
    [DataRow("black and white", true)]
    [DataRow("Color", false)]
    public void Get_WithKeyword_SetsIsGrayScale(string keyword, bool expected) {
        var file = this.CreateFile(new TestJpegBuilder().WithKeyword("Sport").WithKeyword(keyword));

        var actual = this.target.Get(file);

        Assert.AreEqual(expected, actual.IsGrayScale);
    }


    [TestMethod]
    public void Get_WithIptcDateAndTime_ReturnsTimestamp() {
        var file = this.CreateFile(new TestJpegBuilder()
            .WithDateCreated("20260315")
            .WithTimeCreated("143005+0100"));

        var actual = this.target.Get(file);

        Assert.AreEqual(new DateTimeOffset(2026, 3, 15, 14, 30, 5, TimeSpan.FromHours(1)), actual.Timestamp);
    }


    [TestMethod]
    public void Get_WithIptcButNoXmp_LeavesRatingAndLabelEmpty() {
        var file = this.CreateFile(new TestJpegBuilder().WithObjectName("NK Pool"));

        var actual = this.target.Get(file);

        Assert.IsNull(actual.Rating);
        Assert.IsNull(actual.Label);
        Assert.IsFalse(actual.IsPrivate);
    }

    #endregion

    #region XMP

    [TestMethod]
    public void Get_WithXmp_ReadsRatingAndLowercaseLabel() {
        var file = this.CreateFile(new TestJpegBuilder()
            .WithObjectName("NK Pool")
            .WithXmp(rating: 4, label: "Green"));

        var actual = this.target.Get(file);

        Assert.AreEqual(4, actual.Rating);
        Assert.AreEqual("green", actual.Label);
    }


    [TestMethod]
    [DataRow("Red", true)]
    [DataRow("red", true)]
    [DataRow("Yellow", false)]
    public void Get_WithXmpLabel_SetsIsPrivate(string label, bool expected) {
        var file = this.CreateFile(new TestJpegBuilder()
            .WithObjectName("NK Pool")
            .WithXmp(label: label));

        var actual = this.target.Get(file);

        Assert.AreEqual(expected, actual.IsPrivate);
    }


    [TestMethod]
    public void Get_WithXmpLabelButNoRating_ReturnsLabelAndNoRating() {
        var file = this.CreateFile(new TestJpegBuilder()
            .WithObjectName("NK Pool")
            .WithXmp(label: "Red"));

        var actual = this.target.Get(file);

        Assert.IsNull(actual.Rating);
        Assert.AreEqual("red", actual.Label);
        Assert.IsTrue(actual.IsPrivate);
    }


    [TestMethod]
    public void Get_WithXmpWithoutLabel_IsNotPrivate() {
        var file = this.CreateFile(new TestJpegBuilder()
            .WithObjectName("NK Pool")
            .WithXmp(rating: 5));

        var actual = this.target.Get(file);

        Assert.AreEqual(5, actual.Rating);
        Assert.IsNull(actual.Label);
        Assert.IsFalse(actual.IsPrivate);
    }

    #endregion
}
