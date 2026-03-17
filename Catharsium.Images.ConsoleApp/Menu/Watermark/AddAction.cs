using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Watermarker.Interfaces.Menu;

namespace Catharsium.Images.ConsoleApp.Menu.Watermark;

public class AddAction(IWatermarkApplicator watermarkApplicator) 
    : IWatermarkActionHandler
{
    public string MenuName => "Add";

    public async Task Run() {
    }
}