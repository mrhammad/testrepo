using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Models;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly LocalizationService _localization;
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly ProjectsViewModel _projectsViewModel;
    private readonly ContactsViewModel _contactsViewModel;
    private readonly ProductsViewModel _productsViewModel;
    private readonly PurchasesViewModel _purchasesViewModel;
    private readonly SalesViewModel _salesViewModel;
    private readonly StockViewModel _stockViewModel;
    private readonly SettingsViewModel _settingsViewModel;
    private NavigationItem? _selectedModule;
    private object? _currentViewModel;
    private string _pageTitle = string.Empty;
    private string _pageSubtitle = string.Empty;
    private readonly PlaceholderModuleViewModel _paymentsPlaceholder;
    private readonly PlaceholderModuleViewModel _expensesPlaceholder;
    private readonly PlaceholderModuleViewModel _reportsPlaceholder;

    public MainViewModel(IAppDataService dataService, LocalizationService localization)
    {
        _localization = localization;
        _projectsViewModel = new ProjectsViewModel(dataService);
        _contactsViewModel = new ContactsViewModel(dataService);
        _productsViewModel = new ProductsViewModel(dataService);
        _purchasesViewModel = new PurchasesViewModel(dataService);
        _salesViewModel = new SalesViewModel(dataService);
        _stockViewModel = new StockViewModel(dataService);
        _dashboardViewModel = new DashboardViewModel(dataService);
        _settingsViewModel = new SettingsViewModel(localization);
        _paymentsPlaceholder = new PlaceholderModuleViewModel(localization, "nav.payments", "placeholder.next-iteration");
        _expensesPlaceholder = new PlaceholderModuleViewModel(localization, "nav.expenses", "placeholder.next-iteration");
        _reportsPlaceholder = new PlaceholderModuleViewModel(localization, "nav.reports", "placeholder.next-iteration");

        Modules = new ObservableCollection<NavigationItem>
        {
            new NavigationItem(localization, "nav.dashboard", "nav.dashboard.subtitle", _dashboardViewModel),
            new NavigationItem(localization, "nav.projects", "nav.projects.subtitle", _projectsViewModel),
            new NavigationItem(localization, "nav.contacts", "nav.contacts.subtitle", _contactsViewModel),
            new NavigationItem(localization, "nav.products", "nav.products.subtitle", _productsViewModel),
            new NavigationItem(localization, "nav.purchases", "nav.purchases.subtitle", _purchasesViewModel),
            new NavigationItem(localization, "nav.sales", "nav.sales.subtitle", _salesViewModel),
            new NavigationItem(localization, "nav.payments", "nav.payments.subtitle", _paymentsPlaceholder),
            new NavigationItem(localization, "nav.expenses", "nav.expenses.subtitle", _expensesPlaceholder),
            new NavigationItem(localization, "nav.stock", "nav.stock.subtitle", _stockViewModel),
            new NavigationItem(localization, "nav.reports", "nav.reports.subtitle", _reportsPlaceholder),
            new NavigationItem(localization, "nav.settings", "nav.settings.subtitle", _settingsViewModel)
        };

        _projectsViewModel.DataChanged += OnDataChanged;
        _contactsViewModel.DataChanged += OnDataChanged;
        _productsViewModel.DataChanged += OnDataChanged;
        _purchasesViewModel.DataChanged += OnDataChanged;
        _salesViewModel.DataChanged += OnDataChanged;
        _stockViewModel.DataChanged += OnDataChanged;
        _localization.PropertyChanged += HandleLocalizationPropertyChanged;

        _dashboardViewModel.Refresh();
        SelectedModule = Modules[0];
        OnPropertyChanged(nameof(AppTitle));
        OnPropertyChanged(nameof(AppSubtitle));
    }

    public ObservableCollection<NavigationItem> Modules { get; }

    public NavigationItem? SelectedModule
    {
        get => _selectedModule;
        set
        {
            if (!SetProperty(ref _selectedModule, value) || value is null)
            {
                return;
            }

            PageTitle = value.Name;
            PageSubtitle = value.Subtitle;
            CurrentViewModel = value.ViewModel;
        }
    }

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public string PageTitle
    {
        get => _pageTitle;
        private set => SetProperty(ref _pageTitle, value);
    }

    public string PageSubtitle
    {
        get => _pageSubtitle;
        private set => SetProperty(ref _pageSubtitle, value);
    }

    public LocalizationService Localization => _localization;

    public string AppTitle => _localization["app.title"];

    public string AppSubtitle => _localization["app.subtitle"];

    private void OnDataChanged()
    {
        _dashboardViewModel.Refresh();
        _paymentsPlaceholder.LastUpdated = DateTime.Now;
        _expensesPlaceholder.LastUpdated = DateTime.Now;
        _reportsPlaceholder.LastUpdated = DateTime.Now;
    }

    private void HandleLocalizationPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        if (args.PropertyName is not nameof(LocalizationService.CurrentLanguageCode))
        {
            return;
        }

        foreach (var module in Modules)
        {
            module.RefreshText();
        }

        _paymentsPlaceholder.RefreshText();
        _expensesPlaceholder.RefreshText();
        _reportsPlaceholder.RefreshText();

        if (SelectedModule is not null)
        {
            PageTitle = SelectedModule.Name;
            PageSubtitle = SelectedModule.Subtitle;
        }

        OnPropertyChanged(nameof(AppTitle));
        OnPropertyChanged(nameof(AppSubtitle));
    }
}
