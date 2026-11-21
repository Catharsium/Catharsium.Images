using Catharsium.Images.Watermarking._Configuration;

namespace Catharsium.Images.Watermarking.Tests._Configuration;

[TestClass]
public class WatermarkingSettingsTests
{
    [TestMethod]
    public void ResolveImagePath_RelativePathWithLogoFolder_CombinesPaths() {
        var logoFolder = Path.Combine(Path.GetTempPath(), "Logos");
        var settings = new WatermarkingSettings { LogoFolder = logoFolder };

        var actual = settings.ResolveImagePath(Path.Combine("Pool", "Logo.png"));
        Assert.AreEqual(Path.Combine(logoFolder, "Pool", "Logo.png"), actual);
    }


    [TestMethod]
    public void ResolveImagePath_RootedPath_ReturnsPathUnchanged() {
        var settings = new WatermarkingSettings { LogoFolder = Path.Combine(Path.GetTempPath(), "Logos") };
        var rootedPath = Path.Combine(Path.GetTempPath(), "Elsewhere", "Logo.png");

        var actual = settings.ResolveImagePath(rootedPath);
        Assert.AreEqual(rootedPath, actual);
    }


    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void ResolveImagePath_NoLogoFolder_ReturnsPathUnchanged(string logoFolder) {
        var settings = new WatermarkingSettings { LogoFolder = logoFolder };

        var actual = settings.ResolveImagePath("Logo.png");
        Assert.AreEqual("Logo.png", actual);
    }
}
