namespace OrderWiseErp.App.Models;

public sealed class ModuleInfo
{
    public ModuleInfo(string name, string subtitle, IReadOnlyList<string> screens)
    {
        Name = name;
        Subtitle = subtitle;
        Screens = screens;
    }

    public string Name { get; }

    public string Subtitle { get; }

    public IReadOnlyList<string> Screens { get; }
}
