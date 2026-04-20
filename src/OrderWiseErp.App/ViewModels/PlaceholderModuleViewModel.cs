using OrderWiseErp.App.Infrastructure;

namespace OrderWiseErp.App.ViewModels;

public sealed class PlaceholderModuleViewModel : ObservableObject
{
    public PlaceholderModuleViewModel(string moduleName, string note)
    {
        Title = moduleName;
        Message = note;
        LastUpdated = DateTime.Now;
    }

    public string Title { get; }

    public string Message { get; }

    private DateTime _lastUpdated;

    public DateTime LastUpdated
    {
        get => _lastUpdated;
        set => SetProperty(ref _lastUpdated, value);
    }
}
