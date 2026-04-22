using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Models;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class StockViewModel : ObservableObject
{
    private readonly IAppDataService _dataService;

    private readonly ObservableCollection<StockAdjustment> _stockAdjustments = [];
    private readonly ObservableCollection<StockTransfer> _stockTransfers = [];
    private readonly ObservableCollection<StockAdjustmentItem> _draftAdjustmentItems = [];
    private readonly ObservableCollection<StockTransferItem> _draftTransferItems = [];
    private readonly ObservableCollection<string> _partyNames = [];
    private readonly ObservableCollection<string> _locations = [];
    private readonly ObservableCollection<string> _productNames = [];
    private readonly ObservableCollection<string> _productSkus = [];

    private StockAdjustment? _selectedAdjustment;
    private StockAdjustmentItem? _selectedAdjustmentItem;
    private StockTransfer? _selectedTransfer;
    private StockTransferItem? _selectedTransferItem;
    private string _statusMessage = "Ready";

    private DateTime? _listFromDate = DateTime.Today.AddMonths(-1);
    private DateTime? _listToDate = DateTime.Today;
    private string _listPartyFilter = string.Empty;
    private string _listLocationFilter = string.Empty;

    private DateTime _adjustmentDate = DateTime.Today;
    private string _adjustmentReferenceNo = string.Empty;
    private string _adjustmentCustomerOrSupplier = string.Empty;
    private string _adjustmentLocation = string.Empty;
    private string _adjustmentType = "Increase";
    private string _adjustmentTotalRecoveredInput = "0";
    private string _adjustmentReason = string.Empty;
    private string _adjustmentAddedBy = "System";

    private string _adjustmentItemSkuInput = string.Empty;
    private string _adjustmentItemProductNameInput = string.Empty;
    private string _adjustmentItemQtyInput = "0";
    private string _adjustmentItemUnitPriceInput = "0";
    private string _adjustmentItemSubTotalInput = "0";

    private DateTime _transferDate = DateTime.Today;
    private string _transferReferenceNo = string.Empty;
    private string _transferStatus = "Pending";
    private string _transferLocationFrom = string.Empty;
    private string _transferLocationTo = string.Empty;
    private string _transferCustomerOrSupplier = string.Empty;
    private string _transferShippingChargesInput = "0";
    private string _transferAdditionalNote = string.Empty;

    private string _transferItemSkuInput = string.Empty;
    private string _transferItemProductNameInput = string.Empty;
    private string _transferItemQtyInput = "0";
    private string _transferItemUnitPriceInput = "0";
    private string _transferItemSubTotalInput = "0";

    private decimal _adjustmentTotalAmount;
    private decimal _transferTotalAmount;

    public StockViewModel(IAppDataService dataService)
    {
        _dataService = dataService;

        StockAdjustments = new ReadOnlyObservableCollection<StockAdjustment>(_stockAdjustments);
        StockTransfers = new ReadOnlyObservableCollection<StockTransfer>(_stockTransfers);
        DraftAdjustmentItems = new ReadOnlyObservableCollection<StockAdjustmentItem>(_draftAdjustmentItems);
        DraftTransferItems = new ReadOnlyObservableCollection<StockTransferItem>(_draftTransferItems);

        PartyNames = new ReadOnlyObservableCollection<string>(_partyNames);
        Locations = new ReadOnlyObservableCollection<string>(_locations);
        ProductNames = new ReadOnlyObservableCollection<string>(_productNames);
        ProductSkus = new ReadOnlyObservableCollection<string>(_productSkus);

        AdjustmentTypes = new ReadOnlyCollection<string>(["Increase", "Decrease", "Damage", "Correction"]);
        TransferStatuses = new ReadOnlyCollection<string>(["Pending", "In-Transit", "Completed"]);

        NewAdjustmentCommand = new RelayCommand(StartNewAdjustment);
        SaveAdjustmentCommand = new RelayCommand(SaveAdjustment);
        SavePrintAdjustmentCommand = new RelayCommand(SaveAndPrintAdjustment);
        DeleteAdjustmentCommand = new RelayCommand(DeleteAdjustment, () => SelectedAdjustment is not null);
        AddAdjustmentItemCommand = new RelayCommand(AddAdjustmentItem);
        RemoveAdjustmentItemCommand = new RelayCommand(RemoveAdjustmentItem, () => SelectedAdjustmentItem is not null);

        NewTransferCommand = new RelayCommand(StartNewTransfer);
        SaveTransferCommand = new RelayCommand(SaveTransfer);
        SavePrintTransferCommand = new RelayCommand(SaveAndPrintTransfer);
        DeleteTransferCommand = new RelayCommand(DeleteTransfer, () => SelectedTransfer is not null);
        AddTransferItemCommand = new RelayCommand(AddTransferItem);
        RemoveTransferItemCommand = new RelayCommand(RemoveTransferItem, () => SelectedTransferItem is not null);

        RefreshCommand = new RelayCommand(RefreshLists);
        ApplyFiltersCommand = new RelayCommand(RefreshLists);

        RefreshLookups();
        RefreshLists();
        StartNewAdjustment();
        StartNewTransfer();
    }

    public event Action? DataChanged;

    public ReadOnlyObservableCollection<StockAdjustment> StockAdjustments { get; }

    public ReadOnlyObservableCollection<StockTransfer> StockTransfers { get; }

    public ReadOnlyObservableCollection<StockAdjustmentItem> DraftAdjustmentItems { get; }

    public ReadOnlyObservableCollection<StockTransferItem> DraftTransferItems { get; }

    public ReadOnlyObservableCollection<string> PartyNames { get; }

    public ReadOnlyObservableCollection<string> Locations { get; }

    public ReadOnlyObservableCollection<string> ProductNames { get; }

    public ReadOnlyObservableCollection<string> ProductSkus { get; }

    public ReadOnlyCollection<string> AdjustmentTypes { get; }

    public ReadOnlyCollection<string> TransferStatuses { get; }

    public RelayCommand NewAdjustmentCommand { get; }

    public RelayCommand SaveAdjustmentCommand { get; }

    public RelayCommand SavePrintAdjustmentCommand { get; }

    public RelayCommand DeleteAdjustmentCommand { get; }

    public RelayCommand AddAdjustmentItemCommand { get; }

    public RelayCommand RemoveAdjustmentItemCommand { get; }

    public RelayCommand NewTransferCommand { get; }

    public RelayCommand SaveTransferCommand { get; }

    public RelayCommand SavePrintTransferCommand { get; }

    public RelayCommand DeleteTransferCommand { get; }

    public RelayCommand AddTransferItemCommand { get; }

    public RelayCommand RemoveTransferItemCommand { get; }

    public RelayCommand RefreshCommand { get; }

    public RelayCommand ApplyFiltersCommand { get; }

    public StockAdjustment? SelectedAdjustment
    {
        get => _selectedAdjustment;
        set
        {
            if (!SetProperty(ref _selectedAdjustment, value))
            {
                return;
            }

            DeleteAdjustmentCommand.NotifyCanExecuteChanged();
            if (value is null)
            {
                return;
            }

            AdjustmentDate = value.Date;
            AdjustmentReferenceNo = value.ReferenceNo;
            AdjustmentCustomerOrSupplier = value.CustomerOrSupplier;
            AdjustmentLocation = value.Location;
            AdjustmentType = value.AdjustmentType;
            AdjustmentTotalRecoveredInput = value.TotalAmountRecovered.ToString("0.##");
            AdjustmentReason = value.Reason;
            AdjustmentAddedBy = value.AddedBy;

            _draftAdjustmentItems.Clear();
            foreach (var item in value.Items)
            {
                _draftAdjustmentItems.Add(CloneAdjustmentItem(item));
            }

            RecalculateAdjustmentTotals();
            StatusMessage = $"Loaded adjustment {value.ReferenceNo}.";
        }
    }

    public StockAdjustmentItem? SelectedAdjustmentItem
    {
        get => _selectedAdjustmentItem;
        set
        {
            if (!SetProperty(ref _selectedAdjustmentItem, value))
            {
                return;
            }

            RemoveAdjustmentItemCommand.NotifyCanExecuteChanged();
        }
    }

    public StockTransfer? SelectedTransfer
    {
        get => _selectedTransfer;
        set
        {
            if (!SetProperty(ref _selectedTransfer, value))
            {
                return;
            }

            DeleteTransferCommand.NotifyCanExecuteChanged();
            if (value is null)
            {
                return;
            }

            TransferDate = value.Date;
            TransferReferenceNo = value.ReferenceNo;
            TransferStatus = value.Status;
            TransferLocationFrom = value.LocationFrom;
            TransferLocationTo = value.LocationTo;
            TransferCustomerOrSupplier = value.CustomerOrSupplier;
            TransferShippingChargesInput = value.ShippingCharges.ToString("0.##");
            TransferAdditionalNote = value.AdditionalNote;

            _draftTransferItems.Clear();
            foreach (var item in value.Items)
            {
                _draftTransferItems.Add(CloneTransferItem(item));
            }

            RecalculateTransferTotals();
            StatusMessage = $"Loaded transfer {value.ReferenceNo}.";
        }
    }

    public StockTransferItem? SelectedTransferItem
    {
        get => _selectedTransferItem;
        set
        {
            if (!SetProperty(ref _selectedTransferItem, value))
            {
                return;
            }

            RemoveTransferItemCommand.NotifyCanExecuteChanged();
        }
    }

    public DateTime? ListFromDate
    {
        get => _listFromDate;
        set => SetProperty(ref _listFromDate, value);
    }

    public DateTime? ListToDate
    {
        get => _listToDate;
        set => SetProperty(ref _listToDate, value);
    }

    public string ListPartyFilter
    {
        get => _listPartyFilter;
        set => SetProperty(ref _listPartyFilter, value);
    }

    public string ListLocationFilter
    {
        get => _listLocationFilter;
        set => SetProperty(ref _listLocationFilter, value);
    }

    public DateTime AdjustmentDate
    {
        get => _adjustmentDate;
        set => SetProperty(ref _adjustmentDate, value);
    }

    public string AdjustmentReferenceNo
    {
        get => _adjustmentReferenceNo;
        set => SetProperty(ref _adjustmentReferenceNo, value);
    }

    public string AdjustmentCustomerOrSupplier
    {
        get => _adjustmentCustomerOrSupplier;
        set => SetProperty(ref _adjustmentCustomerOrSupplier, value);
    }

    public string AdjustmentLocation
    {
        get => _adjustmentLocation;
        set => SetProperty(ref _adjustmentLocation, value);
    }

    public string AdjustmentType
    {
        get => _adjustmentType;
        set => SetProperty(ref _adjustmentType, value);
    }

    public string AdjustmentTotalRecoveredInput
    {
        get => _adjustmentTotalRecoveredInput;
        set => SetProperty(ref _adjustmentTotalRecoveredInput, value);
    }

    public string AdjustmentReason
    {
        get => _adjustmentReason;
        set => SetProperty(ref _adjustmentReason, value);
    }

    public string AdjustmentAddedBy
    {
        get => _adjustmentAddedBy;
        set => SetProperty(ref _adjustmentAddedBy, value);
    }

    public string AdjustmentItemSkuInput
    {
        get => _adjustmentItemSkuInput;
        set => SetProperty(ref _adjustmentItemSkuInput, value);
    }

    public string AdjustmentItemProductNameInput
    {
        get => _adjustmentItemProductNameInput;
        set => SetProperty(ref _adjustmentItemProductNameInput, value);
    }

    public string AdjustmentItemQtyInput
    {
        get => _adjustmentItemQtyInput;
        set => SetProperty(ref _adjustmentItemQtyInput, value);
    }

    public string AdjustmentItemUnitPriceInput
    {
        get => _adjustmentItemUnitPriceInput;
        set => SetProperty(ref _adjustmentItemUnitPriceInput, value);
    }

    public string AdjustmentItemSubTotalInput
    {
        get => _adjustmentItemSubTotalInput;
        private set => SetProperty(ref _adjustmentItemSubTotalInput, value);
    }

    public DateTime TransferDate
    {
        get => _transferDate;
        set => SetProperty(ref _transferDate, value);
    }

    public string TransferReferenceNo
    {
        get => _transferReferenceNo;
        set => SetProperty(ref _transferReferenceNo, value);
    }

    public string TransferStatus
    {
        get => _transferStatus;
        set => SetProperty(ref _transferStatus, value);
    }

    public string TransferLocationFrom
    {
        get => _transferLocationFrom;
        set => SetProperty(ref _transferLocationFrom, value);
    }

    public string TransferLocationTo
    {
        get => _transferLocationTo;
        set => SetProperty(ref _transferLocationTo, value);
    }

    public string TransferCustomerOrSupplier
    {
        get => _transferCustomerOrSupplier;
        set => SetProperty(ref _transferCustomerOrSupplier, value);
    }

    public string TransferShippingChargesInput
    {
        get => _transferShippingChargesInput;
        set => SetProperty(ref _transferShippingChargesInput, value);
    }

    public string TransferAdditionalNote
    {
        get => _transferAdditionalNote;
        set => SetProperty(ref _transferAdditionalNote, value);
    }

    public string TransferItemSkuInput
    {
        get => _transferItemSkuInput;
        set => SetProperty(ref _transferItemSkuInput, value);
    }

    public string TransferItemProductNameInput
    {
        get => _transferItemProductNameInput;
        set => SetProperty(ref _transferItemProductNameInput, value);
    }

    public string TransferItemQtyInput
    {
        get => _transferItemQtyInput;
        set => SetProperty(ref _transferItemQtyInput, value);
    }

    public string TransferItemUnitPriceInput
    {
        get => _transferItemUnitPriceInput;
        set => SetProperty(ref _transferItemUnitPriceInput, value);
    }

    public string TransferItemSubTotalInput
    {
        get => _transferItemSubTotalInput;
        private set => SetProperty(ref _transferItemSubTotalInput, value);
    }

    public decimal AdjustmentTotalAmount
    {
        get => _adjustmentTotalAmount;
        private set => SetProperty(ref _adjustmentTotalAmount, value);
    }

    public decimal TransferTotalAmount
    {
        get => _transferTotalAmount;
        private set => SetProperty(ref _transferTotalAmount, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    private void RefreshLookups()
    {
        var contacts = _dataService.GetContacts();
        var products = _dataService.GetProducts();

        var names = contacts
            .Select(x => x.BusinessName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        ReplaceCollection(_partyNames, names);

        var locationNames = products
            .Select(x => x.BinLocation)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        if (!locationNames.Any())
        {
            locationNames = ["Main Warehouse", "Secondary Warehouse", "Dispatch Hub"];
        }
        ReplaceCollection(_locations, locationNames);

        var productNames = products
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        ReplaceCollection(_productNames, productNames);

        var skus = products
            .Select(x => x.Sku)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        ReplaceCollection(_productSkus, skus);
    }

    private void RefreshLists()
    {
        _stockAdjustments.Clear();
        foreach (var row in _dataService.GetStockAdjustments().Where(MatchesAdjustmentFilters))
        {
            _stockAdjustments.Add(row);
        }

        _stockTransfers.Clear();
        foreach (var row in _dataService.GetStockTransfers().Where(MatchesTransferFilters))
        {
            _stockTransfers.Add(row);
        }
    }

    private bool MatchesAdjustmentFilters(StockAdjustment item)
    {
        if (ListFromDate.HasValue && item.Date.Date < ListFromDate.Value.Date)
        {
            return false;
        }

        if (ListToDate.HasValue && item.Date.Date > ListToDate.Value.Date)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(ListPartyFilter) &&
            !ContainsIgnoreCase(item.CustomerOrSupplier, ListPartyFilter))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(ListLocationFilter) &&
            !ContainsIgnoreCase(item.Location, ListLocationFilter))
        {
            return false;
        }

        return true;
    }

    private bool MatchesTransferFilters(StockTransfer item)
    {
        if (ListFromDate.HasValue && item.Date.Date < ListFromDate.Value.Date)
        {
            return false;
        }

        if (ListToDate.HasValue && item.Date.Date > ListToDate.Value.Date)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(ListPartyFilter) &&
            !ContainsIgnoreCase(item.CustomerOrSupplier, ListPartyFilter))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(ListLocationFilter) &&
            !ContainsIgnoreCase(item.LocationFrom, ListLocationFilter) &&
            !ContainsIgnoreCase(item.LocationTo, ListLocationFilter))
        {
            return false;
        }

        return true;
    }

    private void StartNewAdjustment()
    {
        SelectedAdjustment = null;
        AdjustmentDate = DateTime.Today;
        AdjustmentReferenceNo = BuildAutoReference("SA");
        AdjustmentCustomerOrSupplier = PartyNames.FirstOrDefault() ?? string.Empty;
        AdjustmentLocation = Locations.FirstOrDefault() ?? string.Empty;
        AdjustmentType = AdjustmentTypes.FirstOrDefault() ?? "Increase";
        AdjustmentTotalRecoveredInput = "0";
        AdjustmentReason = string.Empty;
        AdjustmentAddedBy = "System";
        _draftAdjustmentItems.Clear();
        SelectedAdjustmentItem = null;
        ResetAdjustmentItemInputs();
        RecalculateAdjustmentTotals();
    }

    private void AddAdjustmentItem()
    {
        if (!TryParseDecimal(AdjustmentItemQtyInput, out var qty) ||
            !TryParseDecimal(AdjustmentItemUnitPriceInput, out var unitPrice))
        {
            StatusMessage = "Invalid adjustment item numeric input.";
            return;
        }

        if (qty == 0m || unitPrice < 0m)
        {
            StatusMessage = "Qty cannot be zero and unit price must be non-negative.";
            return;
        }

        var products = _dataService.GetProducts();
        var product = products.FirstOrDefault(x =>
            string.Equals(x.Sku, AdjustmentItemSkuInput.Trim(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.Name, AdjustmentItemProductNameInput.Trim(), StringComparison.OrdinalIgnoreCase));

        var item = new StockAdjustmentItem
        {
            ProductId = product?.Id ?? 0,
            Sku = ResolveSku(product?.Sku, AdjustmentItemSkuInput),
            ProductName = ResolveProductName(product?.Name, AdjustmentItemProductNameInput),
            Qty = qty,
            UnitPrice = unitPrice,
            SubTotal = qty * unitPrice
        };
        _draftAdjustmentItems.Add(item);
        RecalculateAdjustmentTotals();
        ResetAdjustmentItemInputs();
        StatusMessage = "Adjustment line item added.";
    }

    private void RemoveAdjustmentItem()
    {
        if (SelectedAdjustmentItem is null)
        {
            return;
        }

        _draftAdjustmentItems.Remove(SelectedAdjustmentItem);
        SelectedAdjustmentItem = null;
        RecalculateAdjustmentTotals();
        StatusMessage = "Adjustment line item removed.";
    }

    private void SaveAdjustment()
    {
        if (!ValidateAdjustmentHeader(out var totalRecovered))
        {
            return;
        }

        if (_draftAdjustmentItems.Count == 0)
        {
            StatusMessage = "Add at least one stock adjustment item.";
            return;
        }

        var model = new StockAdjustment
        {
            Id = SelectedAdjustment?.Id ?? 0,
            Date = AdjustmentDate,
            ReferenceNo = AdjustmentReferenceNo.Trim(),
            CustomerOrSupplier = AdjustmentCustomerOrSupplier.Trim(),
            Location = AdjustmentLocation.Trim(),
            AdjustmentType = AdjustmentType.Trim(),
            TotalAmountRecovered = totalRecovered,
            Reason = AdjustmentReason.Trim(),
            AddedBy = AdjustmentAddedBy.Trim(),
            Items = _draftAdjustmentItems.Select(CloneAdjustmentItem).ToList()
        };
        model.RecalculateTotals();

        if (model.Id == 0)
        {
            _dataService.AddStockAdjustment(model);
            StatusMessage = $"Stock adjustment {model.ReferenceNo} saved.";
        }
        else
        {
            _dataService.UpdateStockAdjustment(model);
            StatusMessage = $"Stock adjustment {model.ReferenceNo} updated.";
        }

        RefreshLookups();
        RefreshLists();
        DataChanged?.Invoke();
        StartNewAdjustment();
    }

    private void SaveAndPrintAdjustment()
    {
        SaveAdjustment();
        if (!StatusMessage.Contains("saved", StringComparison.OrdinalIgnoreCase) &&
            !StatusMessage.Contains("updated", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        StatusMessage += " (Print action can be wired to report output in next step.)";
    }

    private void DeleteAdjustment()
    {
        if (SelectedAdjustment is null)
        {
            return;
        }

        _dataService.DeleteStockAdjustment(SelectedAdjustment.Id);
        StatusMessage = $"Deleted stock adjustment {SelectedAdjustment.ReferenceNo}.";
        RefreshLists();
        DataChanged?.Invoke();
        StartNewAdjustment();
    }

    private void StartNewTransfer()
    {
        SelectedTransfer = null;
        TransferDate = DateTime.Today;
        TransferReferenceNo = BuildAutoReference("ST");
        TransferStatus = TransferStatuses.FirstOrDefault() ?? "Pending";
        TransferLocationFrom = Locations.FirstOrDefault() ?? string.Empty;
        TransferLocationTo = Locations.Skip(1).FirstOrDefault() ?? Locations.FirstOrDefault() ?? string.Empty;
        TransferCustomerOrSupplier = PartyNames.FirstOrDefault() ?? string.Empty;
        TransferShippingChargesInput = "0";
        TransferAdditionalNote = string.Empty;
        _draftTransferItems.Clear();
        SelectedTransferItem = null;
        ResetTransferItemInputs();
        RecalculateTransferTotals();
    }

    private void AddTransferItem()
    {
        if (!TryParseDecimal(TransferItemQtyInput, out var qty) ||
            !TryParseDecimal(TransferItemUnitPriceInput, out var unitPrice))
        {
            StatusMessage = "Invalid transfer item numeric input.";
            return;
        }

        if (qty <= 0m || unitPrice < 0m)
        {
            StatusMessage = "Transfer qty must be > 0 and unit price must be non-negative.";
            return;
        }

        var products = _dataService.GetProducts();
        var product = products.FirstOrDefault(x =>
            string.Equals(x.Sku, TransferItemSkuInput.Trim(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.Name, TransferItemProductNameInput.Trim(), StringComparison.OrdinalIgnoreCase));

        var item = new StockTransferItem
        {
            ProductId = product?.Id ?? 0,
            Sku = ResolveSku(product?.Sku, TransferItemSkuInput),
            ProductName = ResolveProductName(product?.Name, TransferItemProductNameInput),
            Qty = qty,
            UnitPrice = unitPrice,
            SubTotal = qty * unitPrice
        };
        _draftTransferItems.Add(item);
        RecalculateTransferTotals();
        ResetTransferItemInputs();
        StatusMessage = "Transfer line item added.";
    }

    private void RemoveTransferItem()
    {
        if (SelectedTransferItem is null)
        {
            return;
        }

        _draftTransferItems.Remove(SelectedTransferItem);
        SelectedTransferItem = null;
        RecalculateTransferTotals();
        StatusMessage = "Transfer line item removed.";
    }

    private void SaveTransfer()
    {
        if (!TryParseDecimal(TransferShippingChargesInput, out var shippingCharges))
        {
            StatusMessage = "Shipping charges must be a valid number.";
            return;
        }

        if (string.IsNullOrWhiteSpace(TransferReferenceNo) ||
            string.IsNullOrWhiteSpace(TransferLocationFrom) ||
            string.IsNullOrWhiteSpace(TransferLocationTo))
        {
            StatusMessage = "Reference, location from, and location to are required for stock transfer.";
            return;
        }

        if (_draftTransferItems.Count == 0)
        {
            StatusMessage = "Add at least one stock transfer item.";
            return;
        }

        var model = new StockTransfer
        {
            Id = SelectedTransfer?.Id ?? 0,
            Date = TransferDate,
            ReferenceNo = TransferReferenceNo.Trim(),
            Status = TransferStatus.Trim(),
            LocationFrom = TransferLocationFrom.Trim(),
            LocationTo = TransferLocationTo.Trim(),
            CustomerOrSupplier = TransferCustomerOrSupplier.Trim(),
            ShippingCharges = shippingCharges,
            AdditionalNote = TransferAdditionalNote.Trim(),
            Items = _draftTransferItems.Select(CloneTransferItem).ToList()
        };
        model.RecalculateTotals();

        if (model.Id == 0)
        {
            _dataService.AddStockTransfer(model);
            StatusMessage = $"Stock transfer {model.ReferenceNo} saved.";
        }
        else
        {
            _dataService.UpdateStockTransfer(model);
            StatusMessage = $"Stock transfer {model.ReferenceNo} updated.";
        }

        RefreshLists();
        DataChanged?.Invoke();
        StartNewTransfer();
    }

    private void SaveAndPrintTransfer()
    {
        SaveTransfer();
        if (!StatusMessage.Contains("saved", StringComparison.OrdinalIgnoreCase) &&
            !StatusMessage.Contains("updated", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        StatusMessage += " (Print action can be wired to report output in next step.)";
    }

    private void DeleteTransfer()
    {
        if (SelectedTransfer is null)
        {
            return;
        }

        _dataService.DeleteStockTransfer(SelectedTransfer.Id);
        StatusMessage = $"Deleted stock transfer {SelectedTransfer.ReferenceNo}.";
        RefreshLists();
        DataChanged?.Invoke();
        StartNewTransfer();
    }

    private void RecalculateAdjustmentTotals()
    {
        foreach (var item in _draftAdjustmentItems)
        {
            item.SubTotal = item.Qty * item.UnitPrice;
        }

        AdjustmentTotalAmount = _draftAdjustmentItems.Sum(x => x.SubTotal);
        if (TryParseDecimal(AdjustmentItemQtyInput, out var qty) &&
            TryParseDecimal(AdjustmentItemUnitPriceInput, out var unitPrice))
        {
            AdjustmentItemSubTotalInput = (qty * unitPrice).ToString("0.##");
        }
        else
        {
            AdjustmentItemSubTotalInput = "0";
        }
    }

    private void RecalculateTransferTotals()
    {
        foreach (var item in _draftTransferItems)
        {
            item.SubTotal = item.Qty * item.UnitPrice;
        }

        var linesTotal = _draftTransferItems.Sum(x => x.SubTotal);
        var shipping = TryParseDecimal(TransferShippingChargesInput, out var shippingCharges)
            ? shippingCharges
            : 0m;
        TransferTotalAmount = linesTotal + shipping;

        if (TryParseDecimal(TransferItemQtyInput, out var qty) &&
            TryParseDecimal(TransferItemUnitPriceInput, out var unitPrice))
        {
            TransferItemSubTotalInput = (qty * unitPrice).ToString("0.##");
        }
        else
        {
            TransferItemSubTotalInput = "0";
        }
    }

    private bool ValidateAdjustmentHeader(out decimal totalRecovered)
    {
        totalRecovered = 0m;
        if (!TryParseDecimal(AdjustmentTotalRecoveredInput, out totalRecovered))
        {
            StatusMessage = "Total amount recovered must be a valid number.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(AdjustmentReferenceNo) ||
            string.IsNullOrWhiteSpace(AdjustmentLocation) ||
            string.IsNullOrWhiteSpace(AdjustmentType))
        {
            StatusMessage = "Reference no, location, and adjustment type are required.";
            return false;
        }

        return true;
    }

    private void ResetAdjustmentItemInputs()
    {
        AdjustmentItemSkuInput = ProductSkus.FirstOrDefault() ?? string.Empty;
        AdjustmentItemProductNameInput = ProductNames.FirstOrDefault() ?? string.Empty;
        AdjustmentItemQtyInput = "0";
        AdjustmentItemUnitPriceInput = "0";
        AdjustmentItemSubTotalInput = "0";
    }

    private void ResetTransferItemInputs()
    {
        TransferItemSkuInput = ProductSkus.FirstOrDefault() ?? string.Empty;
        TransferItemProductNameInput = ProductNames.FirstOrDefault() ?? string.Empty;
        TransferItemQtyInput = "0";
        TransferItemUnitPriceInput = "0";
        TransferItemSubTotalInput = "0";
    }

    private static string BuildAutoReference(string prefix)
    {
        return $"{prefix}-{DateTime.Now:yyyyMMddHHmmss}".Substring(0, 17);
    }

    private static string ResolveSku(string? productSku, string inputSku)
    {
        if (!string.IsNullOrWhiteSpace(inputSku))
        {
            return inputSku.Trim();
        }

        return productSku ?? string.Empty;
    }

    private static string ResolveProductName(string? productName, string inputName)
    {
        if (!string.IsNullOrWhiteSpace(inputName))
        {
            return inputName.Trim();
        }

        return productName ?? string.Empty;
    }

    private static bool ContainsIgnoreCase(string? value, string term)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.Contains(term, StringComparison.OrdinalIgnoreCase);
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

    private static StockAdjustmentItem CloneAdjustmentItem(StockAdjustmentItem item)
    {
        return new StockAdjustmentItem
        {
            Id = item.Id,
            StockAdjustmentId = item.StockAdjustmentId,
            ProductId = item.ProductId,
            Sku = item.Sku,
            ProductName = item.ProductName,
            Qty = item.Qty,
            UnitPrice = item.UnitPrice,
            SubTotal = item.SubTotal
        };
    }

    private static StockTransferItem CloneTransferItem(StockTransferItem item)
    {
        return new StockTransferItem
        {
            Id = item.Id,
            StockTransferId = item.StockTransferId,
            ProductId = item.ProductId,
            Sku = item.Sku,
            ProductName = item.ProductName,
            Qty = item.Qty,
            UnitPrice = item.UnitPrice,
            SubTotal = item.SubTotal
        };
    }

    private static void ReplaceCollection(ObservableCollection<string> target, IEnumerable<string> values)
    {
        target.Clear();
        foreach (var value in values)
        {
            target.Add(value);
        }
    }
}
