using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Console.Menu.Base;
using Catharsium.Watermarker.Interfaces.Menu;

namespace Catharsium.Watermarker.Menu.Apply;

public class WatermarkMenu : BaseMenuActionHandler<IWatermarkActionHandler>
{
    public WatermarkMenu(IEnumerable<IWatermarkActionHandler> actionHandlers, IConsole console)
        : base(actionHandlers, console, "Watermark") {
    }
}