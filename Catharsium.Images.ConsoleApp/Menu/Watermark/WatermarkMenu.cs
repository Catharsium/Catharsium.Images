using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Console.Menu.Base;
using Catharsium.Watermarker.Interfaces.Menu;

namespace Catharsium.Images.ConsoleApp.Menu.Watermark;

public class WatermarkMenu(IEnumerable<IWatermarkActionHandler> actionHandlers, IConsole console) 
    : BaseMenuActionHandler<IWatermarkActionHandler>(actionHandlers, console, "Watermark")
{
}