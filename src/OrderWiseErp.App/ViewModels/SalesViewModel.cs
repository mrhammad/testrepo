using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Models;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class SalesViewModel : ObservableObject
{
    private readonly IAppDataService _dataService;
    private Sale? _selectedSale;
    private SaleItem? _selectedItem;
    private string _statusMessage = "Ready";

    private string _invoiceNo = string.Empty;
    private int? _selectedProjectId;
    private int? _selectedCustomerId;
    private DateTime _saleDate = DateTime.Today;
    private DateTime? _dueDate;
    private int _termDays;
    private string _poNumber = string.Empty;

    private int? _selectedProductId;
    private string _itemDescription = string.Empty;
    private string _qtyInput = "1";
    private string _unitInput = "Nos";
    private string _rateInput = "0";
    private string _discountPercentInput = "0";
    private string _vatRateInput = "5";
    private decimal _subtotal;
    private decimal _discountTotal;
    private decimal _vatTotal;

    public SalesViewModel(IAppDataService dataService)
    {
        _dataService = dataService;
        Sales = new ObservableCollection<Sale>();
        CurrentItems = new ObservableCollection<SaleItem>();
        Projects = new ObservableCollection<Project>();
        Customers = new ObservableCollection<Contact>();
        Products = new ObservableCollection<Product>();

        AddOrUpdateSaleCommand = new RelayCommand(SaveSale);
        NewSaleCommand = new RelayCommand(ClearSaleForm);
        DeleteSaleCommand = new RelayCommand(DeleteSale, () => SelectedSale is not null);

        AddItemCommand = new RelayCommand(AddItem);
        RemoveItemCommand = new RelayCommand(RemoveSelectedItem, () => SelectedItem is not null);
        NewItemCommand = new RelayCommand(ClearItemForm);

        LoadLookups();
        RefreshSales();
        AutoFillDefaults();
    }

    public event Action? DataChanged;

    public ObservableCollection<Sale> Sales { get; }

    public ObservableCollection<SaleItem> CurrentItems { get; }

    public ObservableCollection<Project> Projects { get; }

    public ObservableCollection<Contact> Customers { get; }

    public ObservableCollection<Product> Products { get; }

    public Sale? SelectedSale
    {
        get => _selectedSale;
        set
        {
            if (!SetProperty(ref _selectedSale, value))
            {
                return;
            }

            DeleteSaleCommand.NotifyCanExecuteChanged();
            if (value is null)
            {
                return;
            }

            InvoiceNo = value.InvoiceNo;
            SelectedProjectId = value.ProjectId;
            SelectedCustomerId = value.CustomerId;
            SaleDate = value.Date;
            DueDate = value.DueDate;
            TermDays = value.TermDays;
            PoNumber = value.PoNumber;

            CurrentItems.Clear();
            foreach (var item in value.Items)
            {
                CurrentItems.Add(CloneItem(item));
            }

            RecalculateTotals();
            StatusMessage = $"Loaded {value.InvoiceNo} for edit";
        }
    }

    public SaleItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!SetProperty(ref _selectedItem, value))
            {
                return;
            }

            RemoveItemCommand.NotifyCanExecuteChanged();
        }
    }

    public string InvoiceNo
    {
        get => _invoiceNo;
        set => SetProperty(ref _invoiceNo, value);
    }

    public int? SelectedProjectId
    {
        get => _selectedProjectId;
        set => SetProperty(ref _selectedProjectId, value);
    }

    public int? SelectedCustomerId
    {
        get => _selectedCustomerId;
        set => SetProperty(ref _selectedCustomerId, value);
    }

    public DateTime SaleDate
    {
        get => _saleDate;
        set => SetProperty(ref _saleDate, value);
    }

    public DateTime? DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    public int TermDays
    {
        get => _termDays;
        set => SetProperty(ref _termDays, value);
    }

    public string TermDaysInput
    {
        get => _termDays.ToString();
        set
        {
            if (!int.TryParse(value, out var parsed))
            {
                parsed = 0;
            }

            SetProperty(ref _termDays, parsed);
        }
    }

    public string PoNumber
    {
        get => _poNumber;
        set => SetProperty(ref _poNumber, value);
    }

    public DateTime? DueDateInput
    {
        get => DueDate;
        set => DueDate = value;
    }

    public string PoNumberInput
    {
        get => PoNumber;
        set => PoNumber = value;
    }

    public int? SelectedProductId
    {
        get => _selectedProductId;
        set
        {
            if (!SetProperty(ref _selectedProductId, value))
            {
                return;
            }

            if (value is null)
            {
                return;
            }

            var product = Products.FirstOrDefault(x => x.Id == value.Value);
            if (product is null)
            {
                return;
            }

            UnitInput = product.Unit;
            VatRateInput = product.VatRate.ToString("0.##");
            RateInput = product.SalePrice.ToString("0.##");
            if (string.IsNullOrWhiteSpace(ItemDescription))
            {
                ItemDescription = product.Name;
            }
        }
    }

    public string ItemDescription
    {
        get => _itemDescription;
        set => SetProperty(ref _itemDescription, value);
    }

    public string QtyInput
    {
        get => _qtyInput;
        set => SetProperty(ref _qtyInput, value);
    }

    public string UnitInput
    {
        get => _unitInput;
        set => SetProperty(ref _unitInput, value);
    }

    public string RateInput
    {
        get => _rateInput;
        set => SetProperty(ref _rateInput, value);
    }

    public string DiscountPercentInput
    {
        get => _discountPercentInput;
        set => SetProperty(ref _discountPercentInput, value);
    }

    public string VatRateInput
    {
        get => _vatRateInput;
        set => SetProperty(ref _vatRateInput, value);
    }

    private decimal _totalAmount;
    public decimal TotalAmount
    {
        get => _totalAmount;
        private set => SetProperty(ref _totalAmount, value);
    }

    private decimal _balance;
    public decimal Balance
    {
        get => _balance;
        private set => SetProperty(ref _balance, value);
    }

    public decimal Subtotal
    {
        get => _subtotal;
        private set => SetProperty(ref _subtotal, value);
    }

    public decimal DiscountTotal
    {
        get => _discountTotal;
        private set => SetProperty(ref _discountTotal, value);
    }

    public decimal VatTotal
    {
        get => _vatTotal;
        private set => SetProperty(ref _vatTotal, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public RelayCommand AddOrUpdateSaleCommand { get; }

    public RelayCommand NewSaleCommand { get; }

    public RelayCommand DeleteSaleCommand { get; }

    public RelayCommand AddItemCommand { get; }

    public RelayCommand RemoveItemCommand { get; }

    public RelayCommand NewItemCommand { get; }

    public string TotalSummary =>
        $"Subtotal: {Subtotal:N2} | Discount: {DiscountTotal:N2} | VAT: {VatTotal:N2} | Net: {TotalAmount:N2}";

    private void LoadLookups()
    {
        Projects.Clear();
        foreach (var project in _dataService.GetProjects())
        {
            Projects.Add(project);
        }

        Customers.Clear();
        foreach (var contact in _dataService.GetContacts().Where(x =>
                     string.Equals(x.ContactType, "Customer", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(x.ContactType, "Both", StringComparison.OrdinalIgnoreCase)))
        {
            Customers.Add(contact);
        }

        Products.Clear();
        foreach (var product in _dataService.GetProducts())
        {
            Products.Add(product);
        }
    }

    private void RefreshSales()
    {
        Sales.Clear();
        foreach (var sale in _dataService.GetSales())
        {
            Sales.Add(sale);
        }

        LoadLookups();
        RecalculateTotals();
    }

    private void SaveSale()
    {
        if (string.IsNullOrWhiteSpace(InvoiceNo))
        {
            StatusMessage = "Invoice number is required.";
            return;
        }

        if (SelectedProjectId is null || SelectedCustomerId is null)
        {
            StatusMessage = "Project and customer are required.";
            return;
        }

        if (CurrentItems.Count == 0)
        {
            StatusMessage = "Add at least one item.";
            return;
        }

        var selectedProject = Projects.FirstOrDefault(x => x.Id == SelectedProjectId.Value);
        var selectedCustomer = Customers.FirstOrDefault(x => x.Id == SelectedCustomerId.Value);

        var model = new Sale
        {
            Id = SelectedSale?.Id ?? 0,
            InvoiceNo = InvoiceNo.Trim(),
            ProjectId = SelectedProjectId.Value,
            ProjectNo = selectedProject?.ProjectNo ?? string.Empty,
            CustomerId = SelectedCustomerId.Value,
            CustomerName = selectedCustomer?.BusinessName ?? string.Empty,
            Date = SaleDate,
            DueDate = DueDate,
            TermDays = TermDays,
            PoNumber = PoNumber.Trim(),
            TotalAmount = TotalAmount,
            Balance = Balance,
            Items = CurrentItems.Select(CloneItem).ToList()
        };

        if (model.Id == 0)
        {
            _dataService.AddSale(model);
            StatusMessage = "Sale saved.";
        }
        else
        {
            _dataService.UpdateSale(model);
            StatusMessage = "Sale updated.";
        }

        RefreshSales();
        DataChanged?.Invoke();
        ClearSaleForm();
    }

    private void DeleteSale()
    {
        if (SelectedSale is null)
        {
            return;
        }

        _dataService.DeleteSale(SelectedSale.Id);
        StatusMessage = $"Deleted {SelectedSale.InvoiceNo}";
        RefreshSales();
        DataChanged?.Invoke();
        ClearSaleForm();
    }

    private void AddItem()
    {
        if (SelectedProductId is null)
        {
            StatusMessage = "Select a product.";
            return;
        }

        if (!TryParseDecimal(QtyInput, out var qty) ||
            !TryParseDecimal(RateInput, out var rate) ||
            !TryParseDecimal(DiscountPercentInput, out var discountPercent) ||
            !TryParseDecimal(VatRateInput, out var vatRate))
        {
            StatusMessage = "Invalid numeric values in item fields.";
            return;
        }

        var product = Products.FirstOrDefault(x => x.Id == SelectedProductId.Value);
        if (product is null)
        {
            StatusMessage = "Selected product not found.";
            return;
        }

        var amount = qty * rate;
        var discountAmount = amount * discountPercent / 100m;
        var taxable = amount - discountAmount;
        var vatAmount = taxable * vatRate / 100m;
        var net = taxable + vatAmount;

        CurrentItems.Add(new SaleItem
        {
            ProductId = product.Id,
            Sku = product.Sku,
            ProductName = product.Name,
            Description = ItemDescription.Trim(),
            Qty = qty,
            Unit = string.IsNullOrWhiteSpace(UnitInput) ? "Nos" : UnitInput.Trim(),
            Rate = rate,
            Amount = amount,
            DiscountPercent = discountPercent,
            DiscountAmount = discountAmount,
            VatRate = vatRate,
            VatAmount = vatAmount,
            NetAmount = net
        });

        RecalculateTotals();
        ClearItemForm();
        StatusMessage = "Item added.";
    }

    private void RemoveSelectedItem()
    {
        if (SelectedItem is null)
        {
            return;
        }

        CurrentItems.Remove(SelectedItem);
        SelectedItem = null;
        RecalculateTotals();
        StatusMessage = "Item removed.";
    }

    private void RecalculateTotals()
    {
        Subtotal = CurrentItems.Sum(x => x.Amount);
        DiscountTotal = CurrentItems.Sum(x => x.DiscountAmount);
        VatTotal = CurrentItems.Sum(x => x.VatAmount);
        TotalAmount = CurrentItems.Sum(x => x.NetAmount);
        Balance = TotalAmount;
        OnPropertyChanged(nameof(TotalSummary));
    }

    private void ClearSaleForm()
    {
        SelectedSale = null;
        InvoiceNo = $"SAL-{DateTime.Now:yyyyMMddHHmmss}".Substring(0, 16);
        SelectedProjectId = Projects.FirstOrDefault()?.Id;
        SelectedCustomerId = Customers.FirstOrDefault()?.Id;
        SaleDate = DateTime.Today;
        DueDate = null;
        TermDays = 0;
        PoNumber = string.Empty;
        CurrentItems.Clear();
        RecalculateTotals();
        ClearItemForm();
    }

    private void AutoFillDefaults()
    {
        if (string.IsNullOrWhiteSpace(InvoiceNo))
        {
            InvoiceNo = $"SAL-{DateTime.Now:yyyyMMddHHmmss}".Substring(0, 16);
        }

        SelectedProjectId ??= Projects.FirstOrDefault()?.Id;
        SelectedCustomerId ??= Customers.FirstOrDefault()?.Id;
        SelectedProductId ??= Products.FirstOrDefault()?.Id;
    }

    private void ClearItemForm()
    {
        SelectedProductId = Products.FirstOrDefault()?.Id;
        ItemDescription = string.Empty;
        QtyInput = "1";
        UnitInput = "Nos";
        RateInput = "0";
        DiscountPercentInput = "0";
        VatRateInput = "5";
    }

    private static SaleItem CloneItem(SaleItem item)
    {
        return new SaleItem
        {
            Id = item.Id,
            SaleId = item.SaleId,
            ProductId = item.ProductId,
            Sku = item.Sku,
            ProductName = item.ProductName,
            Description = item.Description,
            Qty = item.Qty,
            Unit = item.Unit,
            Rate = item.Rate,
            Amount = item.Amount,
            DiscountPercent = item.DiscountPercent,
            DiscountAmount = item.DiscountAmount,
            VatRate = item.VatRate,
            VatAmount = item.VatAmount,
            NetAmount = item.NetAmount
        };
    }

    private static bool TryParseDecimal(string value, out decimal result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = 0m;
            return true;
        }

        return decimal.TryParse(value, out result);
    }
}
