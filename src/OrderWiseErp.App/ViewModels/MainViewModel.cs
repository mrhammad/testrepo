using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Models;

namespace OrderWiseErp.App.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private ModuleInfo? _selectedModule;

    public MainViewModel()
    {
        Modules = new ObservableCollection<ModuleInfo>
        {
            new("Dashboard", "Overview and quick metrics", new[]
            {
                "Open Projects",
                "Total Receivable",
                "Total Payable",
                "Input/Output VAT snapshot",
                "Cash and bank positions"
            }),
            new("Projects", "Order-wise setup and tracking", new[]
            {
                "Project list",
                "Add/Edit project",
                "Project ledger view"
            }),
            new("Contacts", "Customer / Supplier / Both", new[]
            {
                "Contact list",
                "Add/Edit contact",
                "Filter by contact type"
            }),
            new("Products", "Catalog with VAT/ADT support", new[]
            {
                "Product list",
                "Add/Edit product",
                "SKU and tax setup"
            }),
            new("Purchases", "Supplier invoices with item lines", new[]
            {
                "Purchase list",
                "Purchase add/edit",
                "Print/preview"
            }),
            new("Sales", "Customer invoices with item lines", new[]
            {
                "Sales list",
                "Sales add/edit",
                "Print/preview"
            }),
            new("Payments", "Receipt/payment and allocation", new[]
            {
                "Payments list",
                "Payment add/edit",
                "Allocation to purchases/sales"
            }),
            new("Expenses", "Project-linked expenses", new[]
            {
                "Expense list",
                "Expense add/edit",
                "VAT-aware expense entries"
            }),
            new("Stock", "Adjustments and transfers", new[]
            {
                "Stock adjustment list/add",
                "Stock transfer list/add",
                "Location-based stock flow"
            }),
            new("Reports", "Phase-1 report placeholders", new[]
            {
                "Cash Flow",
                "Project P&L / Overall P&L",
                "Expense and Sales/Purchase reports",
                "Input/Output VAT report"
            })
        };

        SelectedModule = Modules[0];
    }

    public ObservableCollection<ModuleInfo> Modules { get; }

    public ModuleInfo? SelectedModule
    {
        get => _selectedModule;
        set
        {
            if (!SetProperty(ref _selectedModule, value))
            {
                return;
            }

            OnPropertyChanged(nameof(CurrentTitle));
            OnPropertyChanged(nameof(CurrentSubtitle));
            OnPropertyChanged(nameof(CurrentScreens));
        }
    }

    public string CurrentTitle => SelectedModule?.Name ?? string.Empty;

    public string CurrentSubtitle => SelectedModule?.Subtitle ?? string.Empty;

    public IReadOnlyList<string> CurrentScreens => SelectedModule?.Screens ?? Array.Empty<string>();
}
