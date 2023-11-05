using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Watermarker.Interfaces.Menu;

namespace Catharsium.Watermarker.Menu.Apply;

public class AddAction : IWatermarkActionHandler
{
    private readonly IWatermarkApplicator watermarkApplicator;

    public string MenuName => "Add";



    public AddAction(IWatermarkApplicator watermarkApplicator) {
        this.watermarkApplicator = watermarkApplicator;
    }


    public async Task Run() {
    }
}