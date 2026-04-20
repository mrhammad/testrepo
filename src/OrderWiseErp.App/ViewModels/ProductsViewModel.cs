using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Models;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class ProductsViewModel : ObservableObject
{
    private readonly IAppDataService _dataService;
    private Product? _selectedProduct;
    private string _skuInput = string.Empty;
    private string _nameInput = string.Empty;
    private string _categoryInput = string.Empty;
    private string _typeInput = "Stock Product";
    private string _unitInput = "Nos";
    private string _salePriceInput = string.Empty;
    private string _costInput = string.Empty;
    private string _vatRateInput = "5";
    private string _adtRateInput = "0";
    private string _lowStockLevelInput = string.Empty;
    private string _binLocationInput = string.Empty;
    private string _message = "Ready";

    public ProductsViewModel(IAppDataService dataService)
    {
        _dataService = dataService;

        Products = new ObservableCollection<Product>();
        ProductTypes = new ObservableCollection<string>(new[] { "Service", "Stock Product", "Bundle" });
        Units = new ObservableCollection<string>(new[] { "Nos", "Pack", "Kg", "Ltr" });

        SaveCommand = new RelayCommand(Save);
        NewCommand = new RelayCommand(StartNew);
        DeleteCommand = new RelayCommand(Delete, () => SelectedProduct is not null);

        Refresh();
        StartNew();
    }

    public ObservableCollection<Product> Products { get; }

    public ObservableCollection<string> ProductTypes { get; }

    public ObservableCollection<string> Units { get; }

    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (!SetProperty(ref _selectedProduct, value))
            {
                return;
            }

            if (value is not null)
            {
                SkuInput = value.Sku;
                NameInput = value.Name;
                CategoryInput = value.Category;
                TypeInput = string.IsNullOrWhiteSpace(value.Type) ? ProductTypes[1] : value.Type;
                UnitInput = string.IsNullOrWhiteSpace(value.Unit) ? Units[0] : value.Unit;
                SalePriceInput = value.SalePrice.ToString("0.##");
                CostInput = value.Cost.ToString("0.##");
                VatRateInput = value.VatRate.ToString("0.##");
                AdtRateInput = value.AdtRate.ToString("0.##");
                LowStockLevelInput = value.LowStockLevel.ToString();
                BinLocationInput = value.BinLocation;
            }

            DeleteCommand.NotifyCanExecuteChanged();
        }
    }

    public string Message
    {
        get => _message;
        private set => SetProperty(ref _message, value);
    }

    public string SkuInput
    {
        get => _skuInput;
        set => SetProperty(ref _skuInput, value);
    }

    public string NameInput
    {
        get => _nameInput;
        set => SetProperty(ref _nameInput, value);
    }

    public string CategoryInput
    {
        get => _categoryInput;
        set => SetProperty(ref _categoryInput, value);
    }

    public string TypeInput
    {
        get => _typeInput;
        set => SetProperty(ref _typeInput, value);
    }

    public string UnitInput
    {
        get => _unitInput;
        set => SetProperty(ref _unitInput, value);
    }

    public string SalePriceInput
    {
        get => _salePriceInput;
        set => SetProperty(ref _salePriceInput, value);
    }

    public string CostInput
    {
        get => _costInput;
        set => SetProperty(ref _costInput, value);
    }

    public string VatRateInput
    {
        get => _vatRateInput;
        set => SetProperty(ref _vatRateInput, value);
    }

    public string AdtRateInput
    {
        get => _adtRateInput;
        set => SetProperty(ref _adtRateInput, value);
    }

    public string LowStockLevelInput
    {
        get => _lowStockLevelInput;
        set => SetProperty(ref _lowStockLevelInput, value);
    }

    public string BinLocationInput
    {
        get => _binLocationInput;
        set => SetProperty(ref _binLocationInput, value);
    }

    public event Action? DataChanged;

    public RelayCommand SaveCommand { get; }

    public RelayCommand NewCommand { get; }

    public RelayCommand DeleteCommand { get; }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(SkuInput) || string.IsNullOrWhiteSpace(NameInput))
        {
            Message = "SKU and product name are required.";
            return;
        }

        if (!TryParseDecimal(SalePriceInput, out var salePrice) ||
            !TryParseDecimal(CostInput, out var cost) ||
            !TryParseDecimal(VatRateInput, out var vatRate) ||
            !TryParseDecimal(AdtRateInput, out var adtRate))
        {
            Message = "Sale price, cost, VAT, and ADT must be valid numbers.";
            return;
        }

        if (!TryParseInt(LowStockLevelInput, out var lowStockLevel))
        {
            Message = "Low stock level must be a valid integer.";
            return;
        }

        var model = new Product
        {
            Id = SelectedProduct?.Id ?? 0,
            Sku = SkuInput.Trim(),
            Name = NameInput.Trim(),
            Category = CategoryInput.Trim(),
            Type = TypeInput,
            Unit = UnitInput,
            SalePrice = salePrice,
            Cost = cost,
            VatRate = vatRate,
            AdtRate = adtRate,
            LowStockLevel = lowStockLevel,
            BinLocation = BinLocationInput.Trim()
        };

        if (model.Id == 0)
        {
            _dataService.AddProduct(model);
            Message = "Product added.";
        }
        else
        {
            _dataService.UpdateProduct(model);
            Message = "Product updated.";
        }

        Refresh();
        DataChanged?.Invoke();
        StartNew();
    }

    private void Delete()
    {
        if (SelectedProduct is null)
        {
            return;
        }

        _dataService.DeleteProduct(SelectedProduct.Id);
        Message = "Product deleted.";
        Refresh();
        DataChanged?.Invoke();
        StartNew();
    }

    private void StartNew()
    {
        SelectedProduct = null;
        SkuInput = string.Empty;
        NameInput = string.Empty;
        CategoryInput = string.Empty;
        TypeInput = ProductTypes[1];
        UnitInput = Units[0];
        SalePriceInput = string.Empty;
        CostInput = string.Empty;
        VatRateInput = "5";
        AdtRateInput = "0";
        LowStockLevelInput = string.Empty;
        BinLocationInput = string.Empty;
        DeleteCommand.NotifyCanExecuteChanged();
    }

    private void Refresh()
    {
        Products.Clear();
        foreach (var row in _dataService.GetProducts())
        {
            Products.Add(row);
        }
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
}
