using Catharsium.Images.ConsoleApp._Configuration;
using Catharsium.Images.ConsoleApp.Interfaces;
using Catharsium.Images.ConsoleApp.Menu.Watermark;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Util.IO.Console.Interfaces;
using NSubstitute;

namespace Catharsium.Images.ConsoleApp.Tests.Menu.Watermark;

[TestClass]
public class AddActionTests
{
    private IWatermarkApplicator watermarkApplicator = null!;
    private IInputFileResolver inputFileResolver = null!;
    private IConsole console = null!;
    private ConsoleAppSettings settings = null!;
    private AddAction target = null!;


    [TestInitialize]
    public void Setup() {
        this.watermarkApplicator = Substitute.For<IWatermarkApplicator>();
        this.inputFileResolver = Substitute.For<IInputFileResolver>();
        this.console = Substitute.For<IConsole>();
        this.settings = new ConsoleAppSettings { InputFolder = "configured" };
        this.target = new AddAction(this.watermarkApplicator, this.inputFileResolver, this.console, this.settings);
    }


    [TestMethod]
    public async Task Run_EmptyInput_UsesConfiguredInputFolder() {
        string[] files = ["a.jpg"];
        this.console.AskForText(Arg.Any<string>()).Returns("");
        this.inputFileResolver.Resolve(Arg.Is<IEnumerable<string>>(p => p.Single() == "configured")).Returns(files);

        await this.target.Run();
        this.watermarkApplicator.Received().Apply(files);
    }


    [TestMethod]
    public async Task Run_GivenFolder_UsesGivenFolder() {
        string[] files = ["b.jpg"];
        this.console.AskForText(Arg.Any<string>()).Returns("other");
        this.inputFileResolver.Resolve(Arg.Is<IEnumerable<string>>(p => p.Single() == "other")).Returns(files);

        await this.target.Run();
        this.watermarkApplicator.Received().Apply(files);
    }


    [TestMethod]
    public async Task Run_NoInputAndNoInputFolder_DoesNotApply() {
        this.settings.InputFolder = null;
        this.console.AskForText(Arg.Any<string>()).Returns("");

        await this.target.Run();
        this.inputFileResolver.DidNotReceiveWithAnyArgs().Resolve(default!);
        this.watermarkApplicator.DidNotReceiveWithAnyArgs().Apply(default!);
    }


    [TestMethod]
    public async Task Run_NoFilesFound_DoesNotApply() {
        this.console.AskForText(Arg.Any<string>()).Returns("");
        this.inputFileResolver.Resolve(Arg.Any<IEnumerable<string>>()).Returns([]);

        await this.target.Run();
        this.watermarkApplicator.DidNotReceiveWithAnyArgs().Apply(default!);
    }
}
