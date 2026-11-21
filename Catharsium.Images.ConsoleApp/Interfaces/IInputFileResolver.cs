namespace Catharsium.Images.ConsoleApp.Interfaces;

public interface IInputFileResolver
{
    /// <summary>
    /// Turns a list of file and folder paths into the files to process. Folders contribute their top-level files.
    /// </summary>
    string[] Resolve(IEnumerable<string> paths);
}
