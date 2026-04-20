namespace OrderWiseErp.App.Models;

public sealed class NavigationItem
{
    public NavigationItem(string name, string subtitle, object viewModel)
    {
        Name = name;
        Subtitle = subtitle;
        ViewModel = viewModel;
    }

    public string Name { get; }

    public string Subtitle { get; }

    public object ViewModel { get; }
}
