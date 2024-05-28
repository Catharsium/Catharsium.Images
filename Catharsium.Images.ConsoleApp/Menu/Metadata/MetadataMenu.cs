using Catharsium.Images.ConsoleApp.Interfaces.Menu;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Console.Menu.Base;

namespace Catharsium.Images.ConsoleApp.Menu.Metadata;

public class MetadataMenu : BaseMenuActionHandler<IMetadataActionHandler>
{
    public MetadataMenu(IEnumerable<IMetadataActionHandler> actionHandlers, IConsole console)
        : base(actionHandlers, console, "Metadata") {
    }
}