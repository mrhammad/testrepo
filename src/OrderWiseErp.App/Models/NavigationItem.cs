using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.Models;

public sealed class NavigationItem : ObservableObject
{
    private readonly LocalizationService _localization;
    private readonly string _nameKey;
    private readonly string _subtitleKey;
    private string _name = string.Empty;
    private string _subtitle = string.Empty;

    public NavigationItem(LocalizationService localization, string nameKey, string subtitleKey, object viewModel)
    {
        _localization = localization;
        _nameKey = nameKey;
        _subtitleKey = subtitleKey;
        ViewModel = viewModel;
        RefreshText();
    }

    public string Name
    {
        get => _name;
        private set => SetProperty(ref _name, value);
    }

    public string Subtitle
    {
        get => _subtitle;
        private set => SetProperty(ref _subtitle, value);
    }

    public object ViewModel { get; }

    public void RefreshText()
    {
        Name = _localization[_nameKey];
        Subtitle = _localization[_subtitleKey];
    }
}
