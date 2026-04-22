using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class PlaceholderModuleViewModel : ObservableObject
{
    private readonly LocalizationService _localization;
    private readonly string _moduleKey;
    private readonly string _noteKey;
    private string _title = string.Empty;
    private string _message = string.Empty;

    public PlaceholderModuleViewModel(LocalizationService localization, string moduleKey, string noteKey)
    {
        _localization = localization;
        _moduleKey = moduleKey;
        _noteKey = noteKey;
        _localization.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(LocalizationService.CurrentLanguageCode))
            {
                RefreshText();
            }
        };

        RefreshText();
        LastUpdated = DateTime.Now;
    }

    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    public string Message
    {
        get => _message;
        private set => SetProperty(ref _message, value);
    }

    private DateTime _lastUpdated;

    public DateTime LastUpdated
    {
        get => _lastUpdated;
        set
        {
            if (!SetProperty(ref _lastUpdated, value))
            {
                return;
            }

            OnPropertyChanged(nameof(LastUpdatedLabel));
        }
    }

    public string LastUpdatedLabel => $"{_localization["Common.LastDataChange"]} {LastUpdated:G}";

    public void RefreshText()
    {
        Title = _localization[_moduleKey];
        Message = _localization[_noteKey];
        OnPropertyChanged(nameof(LastUpdatedLabel));
    }
}
