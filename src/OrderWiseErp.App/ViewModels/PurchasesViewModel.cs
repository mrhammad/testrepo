using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Models;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class PurchasesViewModel : ObservableObject
{
    private readonly IAppDataService _dataService;
    private readonly List<Product> _products = [];
    private readonly ObservableCollection<string> _productSkus = [];
    private readonly ObservableCollection<string> _itemSkuSuggestions = [];
    private bool _isSyncingSkuAndProduct;
    private readonly ObservableCollection<string> _supplierNames = [];
    private readonly ObservableCollection<string> _projectNumbers = [];
    private readonly ObservableCollection<string> _productNames = [];

    private Purchase? _selectedPurchase;
    private PurchaseItem? _selectedDraftItem;
    private string _message = "Ready";
    private string _purchaseNoInput = string.Empty;
    private string _projectNoInput = string.Empty;
    private string _supplierNameInput = string.Empty;
    private DateTime _dateInput = DateTime.Today;
    private DateTime? _dueDateInput;
    private string _poNumberInput = string.Empty;
    private string _termDaysInput = string.Empty;
    private string _itemNameInput = string.Empty;
    private string _itemSkuInput = string.Empty;
    private string _itemQtyInput = string.Empty;
    private string _itemRateInput = string.Empty;
    private string _itemUnitInput = "Nos";
    private string _itemDescriptionInput = string.Empty;
    private string _itemVatRateInput = "5";
    private string _itemDutyRateInput = string.Empty;
    private string _itemIncomeTaxRateInput = string.Empty;

    private decimal _subtotal;
    private decimal _vatTotal;
    private decimal _dutyTotal;
    private decimal _incomeTaxTotal;
    private decimal _grandTotal;

    public PurchasesViewModel(IAppDataService dataService)
    {
        _dataService = dataService;
        Purchases = new ObservableCollection<Purchase>();
        DraftItems = new ObservableCollection<PurchaseItem>();
        SupplierNames = new ReadOnlyObservableCollection<string>(_supplierNames);
        ProjectNumbers = new ReadOnlyObservableCollection<string>(_projectNumbers);
        ProductNames = new ReadOnlyObservableCollection<string>(_productNames);
        ProductSkus = new ReadOnlyObservableCollection<string>(_productSkus);
        ItemSkuSuggestions = new ReadOnlyObservableCollection<string>(_itemSkuSuggestions);

        NewCommand = new RelayCommand(StartNew);
        SaveCommand = new RelayCommand(SaveDraft);
        DeleteCommand = new RelayCommand(DeleteSelected, () => SelectedPurchase is not null);
        AddItemCommand = new RelayCommand(AddItem);
        RemoveItemCommand = new RelayCommand(RemoveSelectedItem, () => SelectedDraftItem is not null);

        Refresh();
        StartNew();
    }

    public event Action? DataChanged;

    public ObservableCollection<Purchase> Purchases { get; }

    public ObservableCollection<PurchaseItem> DraftItems { get; }

    public ReadOnlyObservableCollection<string> SupplierNames { get; }

    public ReadOnlyObservableCollection<string> ProjectNumbers { get; }

    public ReadOnlyObservableCollection<string> ProductNames { get; }

    public ReadOnlyObservableCollection<string> ProductSkus { get; }

    public ReadOnlyObservableCollection<string> ItemSkuSuggestions { get; }

    public RelayCommand NewCommand { get; }

    public RelayCommand SaveCommand { get; }

    public RelayCommand DeleteCommand { get; }

    public RelayCommand AddItemCommand { get; }

    public RelayCommand RemoveItemCommand { get; }

    public Purchase? SelectedPurchase
    {
        get => _selectedPurchase;
        set
        {
            if (!SetProperty(ref _selectedPurchase, value))
            {
                return;
            }

            DeleteCommand.NotifyCanExecuteChanged();
            if (value is null)
            {
                return;
            }

            PurchaseNoInput = value.PurchaseNo;
            ProjectNoInput = value.ProjectNo;
            SupplierNameInput = value.SupplierName;
            DateInput = value.Date;
            DueDateInput = value.DueDate;
            PoNumberInput = value.PoNumber;
            TermDaysInput = value.TermDays.ToString();

            DraftItems.Clear();
            foreach (var item in value.Items)
            {
                DraftItems.Add(CloneItem(item));
            }

            RecalculateTotals();
            Message = $"Loaded purchase {value.PurchaseNo} for edit.";
        }
    }

    public PurchaseItem? SelectedDraftItem
    {
        get => _selectedDraftItem;
        set
        {
            if (!SetProperty(ref _selectedDraftItem, value))
            {
                return;
            }

            RemoveItemCommand.NotifyCanExecuteChanged();
        }
    }

    public string PurchaseNoInput
    {
        get => _purchaseNoInput;
        set => SetProperty(ref _purchaseNoInput, value);
    }

    public string ProjectNoInput
    {
        get => _projectNoInput;
        set => SetProperty(ref _projectNoInput, value);
    }

    public string SupplierNameInput
    {
        get => _supplierNameInput;
        set => SetProperty(ref _supplierNameInput, value);
    }

    public DateTime DateInput
    {
        get => _dateInput;
        set => SetProperty(ref _dateInput, value);
    }

    public DateTime? DueDateInput
    {
        get => _dueDateInput;
        set => SetProperty(ref _dueDateInput, value);
    }

    public string PoNumberInput
    {
        get => _poNumberInput;
        set => SetProperty(ref _poNumberInput, value);
    }

    public string TermDaysInput
    {
        get => _termDaysInput;
        set => SetProperty(ref _termDaysInput, value);
    }

    public string ItemNameInput
    {
        get => _itemNameInput;
        set
        {
            if (!SetProperty(ref _itemNameInput, value))
            {
                return;
            }

            SyncSkuFromItemName();
        }
    }

    public string ItemSkuInput
    {
        get => _itemSkuInput;
        set
        {
            if (!SetProperty(ref _itemSkuInput, value))
            {
                return;
            }

            UpdateSkuSuggestions();
            SyncItemNameFromSku();
        }
    }

    public string ItemQtyInput
    {
        get => _itemQtyInput;
        set => SetProperty(ref _itemQtyInput, value);
    }

    public string ItemRateInput
    {
        get => _itemRateInput;
        set => SetProperty(ref _itemRateInput, value);
    }

    public string ItemUnitInput
    {
        get => _itemUnitInput;
        set => SetProperty(ref _itemUnitInput, value);
    }

    public string ItemDescriptionInput
    {
        get => _itemDescriptionInput;
        set => SetProperty(ref _itemDescriptionInput, value);
    }

    public string ItemVatRateInput
    {
        get => _itemVatRateInput;
        set => SetProperty(ref _itemVatRateInput, value);
    }

    public string ItemDutyRateInput
    {
        get => _itemDutyRateInput;
        set => SetProperty(ref _itemDutyRateInput, value);
    }

    public string ItemIncomeTaxRateInput
    {
        get => _itemIncomeTaxRateInput;
        set => SetProperty(ref _itemIncomeTaxRateInput, value);
    }

    public decimal Subtotal
    {
        get => _subtotal;
        private set => SetProperty(ref _subtotal, value);
    }

    public decimal VatTotal
    {
        get => _vatTotal;
        private set => SetProperty(ref _vatTotal, value);
    }

    public decimal DutyTotal
    {
        get => _dutyTotal;
        private set => SetProperty(ref _dutyTotal, value);
    }

    public decimal IncomeTaxTotal
    {
        get => _incomeTaxTotal;
        private set => SetProperty(ref _incomeTaxTotal, value);
    }

    public decimal GrandTotal
    {
        get => _grandTotal;
        private set => SetProperty(ref _grandTotal, value);
    }

    public decimal TotalAmount => GrandTotal;

    public decimal Balance => GrandTotal;

    public string GrandTotalSummary =>
        $"Subtotal: {Subtotal:N2} | VAT: {VatTotal:N2} | Duty: {DutyTotal:N2} | Income Tax: {IncomeTaxTotal:N2} | Total: {GrandTotal:N2}";

    public bool IsItemSkuSuggestionOpen =>
        !string.IsNullOrWhiteSpace(ItemSkuInput) && ItemSkuSuggestions.Count > 0;

    public string Message
    {
        get => _message;
        private set => SetProperty(ref _message, value);
    }

    public void Refresh()
    {
        Purchases.Clear();
        foreach (var row in _dataService.GetPurchases())
        {
            Purchases.Add(row);
        }

        RefreshLookups();
        RecalculateTotals();
    }

    private void RefreshLookups()
    {
        var supplierNames = _dataService.GetContacts()
            .Where(x => x.ContactType is "Supplier" or "Both")
            .Select(x => x.BusinessName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        _supplierNames.Clear();
        foreach (var supplier in supplierNames)
        {
            _supplierNames.Add(supplier);
        }

        var projectNos = _dataService.GetProjects()
            .Select(x => x.ProjectNo)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        _projectNumbers.Clear();
        foreach (var projectNo in projectNos)
        {
            _projectNumbers.Add(projectNo);
        }

        var productNames = _dataService.GetProducts()
            .OrderBy(x => x.Sku)
            .ToList();
        _products.Clear();
        _products.AddRange(productNames);

        var names = productNames
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        _productNames.Clear();
        foreach (var productName in names)
        {
            _productNames.Add(productName);
        }

        var skus = productNames
            .Select(x => x.Sku)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        _productSkus.Clear();
        foreach (var sku in skus)
        {
            _productSkus.Add(sku);
        }

        UpdateSkuSuggestions();
    }

    private void StartNew()
    {
        SelectedPurchase = null;
        PurchaseNoInput = BuildPurchaseNo();
        ProjectNoInput = ProjectNumbers.FirstOrDefault() ?? string.Empty;
        SupplierNameInput = SupplierNames.FirstOrDefault() ?? string.Empty;
        DateInput = DateTime.Today;
        DueDateInput = null;
        PoNumberInput = string.Empty;
        TermDaysInput = string.Empty;
        DraftItems.Clear();
        SelectedDraftItem = null;
        ItemSkuInput = ProductSkus.FirstOrDefault() ?? string.Empty;
        ItemNameInput = ProductNames.FirstOrDefault() ?? string.Empty;
        ItemDescriptionInput = string.Empty;
        ItemQtyInput = string.Empty;
        ItemUnitInput = "Nos";
        ItemRateInput = string.Empty;
        ItemVatRateInput = "5";
        ItemDutyRateInput = string.Empty;
        ItemIncomeTaxRateInput = string.Empty;
        RecalculateTotals();
    }

    private void SaveDraft()
    {
        if (string.IsNullOrWhiteSpace(PurchaseNoInput) || string.IsNullOrWhiteSpace(ProjectNoInput) || string.IsNullOrWhiteSpace(SupplierNameInput))
        {
            Message = "Invoice no, project, and supplier are required.";
            return;
        }

        if (!TryParseInt(TermDaysInput, out var termDays))
        {
            Message = "Term days must be a valid whole number.";
            return;
        }

        if (DraftItems.Count == 0)
        {
            Message = "At least one purchase line item is required.";
            return;
        }

        var projects = _dataService.GetProjects();
        var suppliers = _dataService.GetContacts();
        var project = projects.FirstOrDefault(x =>
            string.Equals(x.ProjectNo, ProjectNoInput.Trim(), StringComparison.OrdinalIgnoreCase));
        var supplier = suppliers.FirstOrDefault(x =>
            (x.ContactType is "Supplier" or "Both") &&
            string.Equals(x.BusinessName, SupplierNameInput.Trim(), StringComparison.OrdinalIgnoreCase));

        var model = new Purchase
        {
            Id = SelectedPurchase?.Id ?? 0,
            PurchaseNo = PurchaseNoInput.Trim(),
            ProjectNo = ProjectNoInput.Trim(),
            ProjectId = project?.Id ?? 0,
            SupplierName = SupplierNameInput.Trim(),
            SupplierId = supplier?.Id ?? 0,
            Date = DateInput,
            DueDate = DueDateInput,
            TermDays = termDays,
            PoNumber = PoNumberInput.Trim(),
            Items = DraftItems.Select(CloneItem).ToList()
        };
        model.RecalculateTotals();

        if (model.Id == 0)
        {
            _dataService.AddPurchase(model);
            Message = "Purchase saved.";
        }
        else
        {
            _dataService.UpdatePurchase(model);
            Message = "Purchase updated.";
        }

        Refresh();
        DataChanged?.Invoke();
        StartNew();
    }

    private void DeleteSelected()
    {
        if (SelectedPurchase is null)
        {
            return;
        }

        _dataService.DeletePurchase(SelectedPurchase.Id);
        Message = $"Deleted purchase {SelectedPurchase.PurchaseNo}.";
        Refresh();
        DataChanged?.Invoke();
        StartNew();
    }

    private void AddItem()
    {
        if (string.IsNullOrWhiteSpace(ItemNameInput))
        {
            Message = "Item name is required.";
            return;
        }

        if (!TryParseDecimal(ItemQtyInput, out var qty) ||
            !TryParseDecimal(ItemRateInput, out var rate) ||
            !TryParseDecimal(ItemVatRateInput, out var vatRate) ||
            !TryParseDecimal(ItemDutyRateInput, out var dutyRate) ||
            !TryParseDecimal(ItemIncomeTaxRateInput, out var incomeTaxRate))
        {
            Message = "Invalid numeric values in line item.";
            return;
        }

        if (qty <= 0 || rate < 0)
        {
            Message = "Qty must be > 0 and rate must be >= 0.";
            return;
        }

        var product = FindProductBySkuOrName(ItemSkuInput, ItemNameInput);
        if (product is null)
        {
            Message = "Select an existing product SKU/name from Products module.";
            return;
        }

        var amount = qty * rate;
        var vatAmount = amount * (vatRate / 100m);
        var dutyAmount = amount * (dutyRate / 100m);
        var incomeTaxAmount = amount * (incomeTaxRate / 100m);
        var netAmount = amount + vatAmount + dutyAmount + incomeTaxAmount;

        DraftItems.Add(
            new PurchaseItem
            {
                ProductId = product.Id,
                Sku = product.Sku,
                ProductName = product.Name,
                Description = string.IsNullOrWhiteSpace(ItemDescriptionInput) ? product.Name : ItemDescriptionInput.Trim(),
                Qty = qty,
                Unit = string.IsNullOrWhiteSpace(ItemUnitInput) ? product.Unit : ItemUnitInput.Trim(),
                Rate = rate,
                Amount = amount,
                CustomDuty = dutyRate,
                VatRate = vatRate,
                VatAmount = vatAmount,
                IncomeTaxRate = incomeTaxRate,
                IncomeTaxAmount = incomeTaxAmount,
                NetAmount = netAmount
            });

        ItemSkuInput = ProductSkus.FirstOrDefault() ?? string.Empty;
        ItemNameInput = ProductNames.FirstOrDefault() ?? string.Empty;
        ItemDescriptionInput = string.Empty;
        ItemQtyInput = string.Empty;
        ItemUnitInput = "Nos";
        ItemRateInput = string.Empty;
        ItemVatRateInput = "5";
        ItemDutyRateInput = string.Empty;
        ItemIncomeTaxRateInput = string.Empty;
        RecalculateTotals();
        Message = "Line item added.";
    }

    private static string BuildPurchaseNo()
    {
        return $"PUR-{DateTime.Now:yyyyMMddHHmmss}".Substring(0, 16);
    }

    private void RemoveSelectedItem()
    {
        if (SelectedDraftItem is null)
        {
            return;
        }

        DraftItems.Remove(SelectedDraftItem);
        SelectedDraftItem = null;
        RecalculateTotals();
        Message = "Line item removed.";
    }

    private void RecalculateTotals()
    {
        Subtotal = DraftItems.Sum(x => x.Amount);
        VatTotal = DraftItems.Sum(x => x.VatAmount);
        DutyTotal = DraftItems.Sum(x => x.CustomDutyAmount);
        IncomeTaxTotal = DraftItems.Sum(x => x.IncomeTaxAmount);
        GrandTotal = DraftItems.Sum(x => x.NetAmount);
        OnPropertyChanged(nameof(GrandTotalSummary));
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

    private static bool TryParseInt(string value, out int result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = 0;
            return true;
        }

        return int.TryParse(value, out result);
    }

    private Product? FindProductBySkuOrName(string skuInput, string nameInput)
    {
        var sku = skuInput.Trim();
        if (!string.IsNullOrWhiteSpace(sku))
        {
            var bySku = _products.FirstOrDefault(x => string.Equals(x.Sku, sku, StringComparison.OrdinalIgnoreCase));
            if (bySku is not null)
            {
                return bySku;
            }
        }

        var name = nameInput.Trim();
        if (!string.IsNullOrWhiteSpace(name))
        {
            return _products.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        return null;
    }

    private void UpdateSkuSuggestions()
    {
        var search = ItemSkuInput.Trim();
        var suggestions = ProductSkus
            .Where(sku => string.IsNullOrWhiteSpace(search) || sku.StartsWith(search, StringComparison.OrdinalIgnoreCase))
            .Take(30)
            .ToList();

        _itemSkuSuggestions.Clear();
        foreach (var suggestion in suggestions)
        {
            _itemSkuSuggestions.Add(suggestion);
        }

        OnPropertyChanged(nameof(IsItemSkuSuggestionOpen));
    }

    private void SyncItemNameFromSku()
    {
        if (_isSyncingSkuAndProduct)
        {
            return;
        }

        var sku = ItemSkuInput.Trim();
        if (string.IsNullOrWhiteSpace(sku))
        {
            return;
        }

        var product = _products.FirstOrDefault(x => string.Equals(x.Sku, sku, StringComparison.OrdinalIgnoreCase));
        if (product is null)
        {
            return;
        }

        _isSyncingSkuAndProduct = true;
        try
        {
            ItemNameInput = product.Name;
        }
        finally
        {
            _isSyncingSkuAndProduct = false;
        }
    }

    private void SyncSkuFromItemName()
    {
        if (_isSyncingSkuAndProduct)
        {
            return;
        }

        var name = ItemNameInput.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var product = _products.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        if (product is null)
        {
            return;
        }

        _isSyncingSkuAndProduct = true;
        try
        {
            ItemSkuInput = product.Sku;
        }
        finally
        {
            _isSyncingSkuAndProduct = false;
        }
    }

    private static PurchaseItem CloneItem(PurchaseItem source)
    {
        return new PurchaseItem
        {
            Id = source.Id,
            ProductId = source.ProductId,
            Sku = source.Sku,
            ProductName = source.ProductName,
            Description = source.Description,
            Qty = source.Qty,
            Unit = source.Unit,
            Rate = source.Rate,
            Amount = source.Amount,
            CustomDuty = source.CustomDuty,
            VatRate = source.VatRate,
            VatAmount = source.VatAmount,
            IncomeTaxRate = source.IncomeTaxRate,
            IncomeTaxAmount = source.IncomeTaxAmount,
            NetAmount = source.NetAmount
        };
    }
}
