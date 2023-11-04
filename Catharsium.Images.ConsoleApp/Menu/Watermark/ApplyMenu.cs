using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Console.Menu.Base;
using Catharsium.Watermarker.Interfaces.Menu;

namespace Catharsium.Watermarker.Menu.Apply;

public class ApplyMenu : BaseMenuActionHandler<IApplyActionHandler>
{
    public ApplyMenu(IEnumerable<IApplyActionHandler> actionHandlers, IConsole console)
        : base(actionHandlers, console, "Apply") {
    }
}