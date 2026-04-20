using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Models;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly ProjectsViewModel _projectsViewModel;
    private readonly ContactsViewModel _contactsViewModel;
    private readonly ProductsViewModel _productsViewModel;
    private readonly PurchasesViewModel _purchasesViewModel;
    private readonly SalesViewModel _salesViewModel;
    private NavigationItem? _selectedModule;
    private object? _currentViewModel;
    private string _pageTitle = string.Empty;
    private string _pageSubtitle = string.Empty;
    private readonly PlaceholderModuleViewModel _paymentsPlaceholder = new("Payments", "Planned in next iteration");
    private readonly PlaceholderModuleViewModel _expensesPlaceholder = new("Expenses", "Planned in next iteration");
    private readonly PlaceholderModuleViewModel _stockPlaceholder = new("Stock", "Planned in next iteration");
    private readonly PlaceholderModuleViewModel _reportsPlaceholder = new("Reports", "Planned in next iteration");

    public MainViewModel(IAppDataService dataService)
    {
        _projectsViewModel = new ProjectsViewModel(dataService);
        _contactsViewModel = new ContactsViewModel(dataService);
        _productsViewModel = new ProductsViewModel(dataService);
        _purchasesViewModel = new PurchasesViewModel(dataService);
        _salesViewModel = new SalesViewModel(dataService);
        _dashboardViewModel = new DashboardViewModel(dataService);

        Modules = new ObservableCollection<NavigationItem>
        {
            new NavigationItem("Dashboard", "Overview and quick metrics", _dashboardViewModel),
            new NavigationItem("Projects", "Create and manage order-wise projects", _projectsViewModel),
            new NavigationItem("Contacts", "Customers, suppliers, and both", _contactsViewModel),
            new NavigationItem("Products", "Products with pricing and tax fields", _productsViewModel),
            new NavigationItem("Purchases", "Purchase invoice entry with item lines", _purchasesViewModel),
            new NavigationItem("Sales", "Sales invoice entry with item lines", _salesViewModel),
            new NavigationItem("Payments", "Phase 2: Payment and allocation", _paymentsPlaceholder),
            new NavigationItem("Expenses", "Phase 2: Expense vouchers", _expensesPlaceholder),
            new NavigationItem("Stock", "Phase 2: Stock transfers and adjustments", _stockPlaceholder),
            new NavigationItem("Reports", "Phase 2: Financial and tax reports", _reportsPlaceholder)
        };

        _projectsViewModel.DataChanged += OnDataChanged;
        _contactsViewModel.DataChanged += OnDataChanged;
        _productsViewModel.DataChanged += OnDataChanged;
        _purchasesViewModel.DataChanged += OnDataChanged;
        _salesViewModel.DataChanged += OnDataChanged;

        _dashboardViewModel.Refresh();
        SelectedModule = Modules[0];
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

    private void OnDataChanged()
    {
        _dashboardViewModel.Refresh();
        _paymentsPlaceholder.LastUpdated = DateTime.Now;
        _expensesPlaceholder.LastUpdated = DateTime.Now;
        _stockPlaceholder.LastUpdated = DateTime.Now;
        _reportsPlaceholder.LastUpdated = DateTime.Now;
    }
}
