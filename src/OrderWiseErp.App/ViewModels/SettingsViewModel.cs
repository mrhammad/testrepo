using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class SettingsViewModel : ObservableObject
{
    private readonly LocalizationService _localizationService;
    private string _selectedLanguageCode;

    public SettingsViewModel(LocalizationService localizationService)
    {
        _localizationService = localizationService;
        AvailableLanguages = new ObservableCollection<LanguageOption>
        {
            new(LocalizationService.EnglishLanguageCode, "Common.English", _localizationService),
            new(LocalizationService.ArabicLanguageCode, "Common.Arabic", _localizationService)
        };

        _selectedLanguageCode = _localizationService.CurrentLanguageCode;
        _localizationService.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(LocalizationService.CurrentLanguageCode))
            {
                _selectedLanguageCode = _localizationService.CurrentLanguageCode;
                OnPropertyChanged(nameof(SelectedLanguageCode));
            }
        };
    }

    public ObservableCollection<LanguageOption> AvailableLanguages { get; }

    public string SelectedLanguageCode
    {
        get => _selectedLanguageCode;
        set
        {
            if (!SetProperty(ref _selectedLanguageCode, value))
            {
                return;
            }

            _localizationService.SetLanguage(value);
        }
    }

    public LocalizationService Localization => _localizationService;
}

public sealed class LanguageOption : ObservableObject
{
    private readonly LocalizationService _localization;

    public LanguageOption(string code, string displayKey, LocalizationService localization)
    {
        Code = code;
        DisplayKey = displayKey;
        _localization = localization;
        _localization.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(LocalizationService.CurrentLanguageCode))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
        };
    }

    public string Code { get; }

    public string DisplayKey { get; }

    public string DisplayName => _localization[DisplayKey];
}
