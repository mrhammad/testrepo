using System.IO;
using System.Text.Json;
using OrderWiseErp.App.Models;

namespace OrderWiseErp.App.Services;

public sealed class JsonFileAppDataService : IAppDataService
{
    private readonly string _storagePath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly AppDataStore _store;

    private int _nextProjectId;
    private int _nextContactId;
    private int _nextProductId;
    private int _nextPurchaseId;
    private int _nextPurchaseItemId;
    private int _nextSaleId;
    private int _nextSaleItemId;

    public JsonFileAppDataService(string storagePath)
    {
        _storagePath = storagePath;
        var directory = Path.GetDirectoryName(_storagePath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException("Storage path must include a directory.");
        }

        Directory.CreateDirectory(directory);
        _store = LoadOrCreateStore(_storagePath);

        SeedIfEmpty(_store);
        RefreshCounters();
        Save();
    }

    public IReadOnlyList<Project> Projects => _store.Projects;

    public IReadOnlyList<Contact> Contacts => _store.Contacts;

    public IReadOnlyList<Product> Products => _store.Products;

    public IReadOnlyList<Purchase> Purchases => _store.Purchases;

    public IReadOnlyList<Sale> Sales => _store.Sales;

    public IReadOnlyList<Project> GetProjects()
    {
        return _store.Projects.OrderBy(x => x.ProjectNo).Select(CloneProject).ToList();
    }

    public IReadOnlyList<Contact> GetContacts()
    {
        return _store.Contacts.OrderBy(x => x.BusinessName).Select(CloneContact).ToList();
    }

    public IReadOnlyList<Product> GetProducts()
    {
        return _store.Products.OrderBy(x => x.Sku).Select(CloneProduct).ToList();
    }

    public IReadOnlyList<Purchase> GetPurchases()
    {
        return _store.Purchases.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).Select(ClonePurchase).ToList();
    }

    public IReadOnlyList<Sale> GetSales()
    {
        return _store.Sales.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).Select(CloneSale).ToList();
    }

    public Project AddProject(Project project)
    {
        var copy = CloneProject(project);
        copy.Id = _nextProjectId++;
        _store.Projects.Add(copy);
        Save();
        return CloneProject(copy);
    }

    public bool UpdateProject(Project project)
    {
        var existing = _store.Projects.FirstOrDefault(x => x.Id == project.Id);
        if (existing is null)
        {
            return false;
        }

        existing.ProjectNo = project.ProjectNo;
        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.StartDate = project.StartDate;
        existing.EndDate = project.EndDate;
        Save();
        return true;
    }

    public bool DeleteProject(int projectId)
    {
        var existing = _store.Projects.FirstOrDefault(x => x.Id == projectId);
        if (existing is null)
        {
            return false;
        }

        if (_store.Purchases.Any(x => x.ProjectId == projectId) || _store.Sales.Any(x => x.ProjectId == projectId))
        {
            return false;
        }

        var removed = _store.Projects.Remove(existing);
        if (removed)
        {
            Save();
        }

        return removed;
    }

    public Contact AddContact(Contact contact)
    {
        var copy = CloneContact(contact);
        copy.Id = _nextContactId++;
        _store.Contacts.Add(copy);
        Save();
        return CloneContact(copy);
    }

    public bool UpdateContact(Contact contact)
    {
        var existing = _store.Contacts.FirstOrDefault(x => x.Id == contact.Id);
        if (existing is null)
        {
            return false;
        }

        existing.ContactType = contact.ContactType;
        existing.ContactCategory = contact.ContactCategory;
        existing.BusinessName = contact.BusinessName;
        existing.Title = contact.Title;
        existing.FirstName = contact.FirstName;
        existing.LastName = contact.LastName;
        existing.Email = contact.Email;
        existing.Mobile = contact.Mobile;
        existing.Phone = contact.Phone;
        existing.AccountNo = contact.AccountNo;
        existing.Website = contact.Website;
        existing.Address = contact.Address;
        existing.City = contact.City;
        existing.Province = contact.Province;
        existing.PostalCode = contact.PostalCode;
        existing.Country = contact.Country;
        existing.BatakaNumber = contact.BatakaNumber;
        existing.BusinessLicenseNumber = contact.BusinessLicenseNumber;
        existing.VatNumber = contact.VatNumber;
        existing.PaymentTermDays = contact.PaymentTermDays;
        existing.CreditLimit = contact.CreditLimit;
        existing.OpeningBalance = contact.OpeningBalance;
        existing.Notes = contact.Notes;
        Save();
        return true;
    }

    public bool DeleteContact(int contactId)
    {
        var existing = _store.Contacts.FirstOrDefault(x => x.Id == contactId);
        if (existing is null)
        {
            return false;
        }

        if (_store.Purchases.Any(x => x.SupplierId == contactId) || _store.Sales.Any(x => x.CustomerId == contactId))
        {
            return false;
        }

        var removed = _store.Contacts.Remove(existing);
        if (removed)
        {
            Save();
        }

        return removed;
    }

    public Product AddProduct(Product product)
    {
        var copy = CloneProduct(product);
        copy.Id = _nextProductId++;
        _store.Products.Add(copy);
        Save();
        return CloneProduct(copy);
    }

    public bool UpdateProduct(Product product)
    {
        var existing = _store.Products.FirstOrDefault(x => x.Id == product.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Sku = product.Sku;
        existing.Name = product.Name;
        existing.Category = product.Category;
        existing.Type = product.Type;
        existing.StockAccount = product.StockAccount;
        existing.LowStockLevel = product.LowStockLevel;
        existing.SalePrice = product.SalePrice;
        existing.Cost = product.Cost;
        existing.SaleDiscount = product.SaleDiscount;
        existing.PurchaseDiscount = product.PurchaseDiscount;
        existing.Weight = product.Weight;
        existing.Unit = product.Unit;
        existing.VatRate = product.VatRate;
        existing.AdtRate = product.AdtRate;
        existing.BinLocation = product.BinLocation;
        existing.Notes = product.Notes;
        Save();
        return true;
    }

    public bool DeleteProduct(int productId)
    {
        var existing = _store.Products.FirstOrDefault(x => x.Id == productId);
        if (existing is null)
        {
            return false;
        }

        if (_store.Purchases.Any(x => x.Items.Any(i => i.ProductId == productId)) ||
            _store.Sales.Any(x => x.Items.Any(i => i.ProductId == productId)))
        {
            return false;
        }

        var removed = _store.Products.Remove(existing);
        if (removed)
        {
            Save();
        }

        return removed;
    }

    public Purchase AddPurchase(Purchase purchase)
    {
        var copy = ClonePurchase(purchase);
        copy.Id = _nextPurchaseId++;
        copy.Items = copy.Items.Select(ClonePurchaseItem).ToList();
        foreach (var item in copy.Items)
        {
            item.Id = _nextPurchaseItemId++;
            item.PurchaseId = copy.Id;
        }

        RecalculatePurchase(copy);
        _store.Purchases.Add(copy);
        Save();
        return ClonePurchase(copy);
    }

    public bool UpdatePurchase(Purchase purchase)
    {
        var existing = _store.Purchases.FirstOrDefault(x => x.Id == purchase.Id);
        if (existing is null)
        {
            return false;
        }

        existing.ProjectId = purchase.ProjectId;
        existing.SupplierId = purchase.SupplierId;
        existing.Date = purchase.Date;
        existing.DueDate = purchase.DueDate;
        existing.TermDays = purchase.TermDays;
        existing.PoNumber = purchase.PoNumber;
        existing.Balance = purchase.Balance;
        existing.Items = purchase.Items.Select(ClonePurchaseItem).ToList();

        foreach (var item in existing.Items)
        {
            if (item.Id <= 0)
            {
                item.Id = _nextPurchaseItemId++;
            }

            item.PurchaseId = existing.Id;
        }

        RecalculatePurchase(existing);
        Save();
        return true;
    }

    public bool DeletePurchase(int purchaseId)
    {
        var existing = _store.Purchases.FirstOrDefault(x => x.Id == purchaseId);
        if (existing is null)
        {
            return false;
        }

        var removed = _store.Purchases.Remove(existing);
        if (removed)
        {
            Save();
        }

        return removed;
    }

    public Sale AddSale(Sale sale)
    {
        var copy = CloneSale(sale);
        copy.Id = _nextSaleId++;
        copy.Items = copy.Items.Select(CloneSaleItem).ToList();
        foreach (var item in copy.Items)
        {
            item.Id = _nextSaleItemId++;
            item.SaleId = copy.Id;
        }

        RecalculateSale(copy);
        _store.Sales.Add(copy);
        Save();
        return CloneSale(copy);
    }

    public bool UpdateSale(Sale sale)
    {
        var existing = _store.Sales.FirstOrDefault(x => x.Id == sale.Id);
        if (existing is null)
        {
            return false;
        }

        existing.ProjectId = sale.ProjectId;
        existing.CustomerId = sale.CustomerId;
        existing.Date = sale.Date;
        existing.DueDate = sale.DueDate;
        existing.TermDays = sale.TermDays;
        existing.PoNumber = sale.PoNumber;
        existing.Balance = sale.Balance;
        existing.Items = sale.Items.Select(CloneSaleItem).ToList();

        foreach (var item in existing.Items)
        {
            if (item.Id <= 0)
            {
                item.Id = _nextSaleItemId++;
            }

            item.SaleId = existing.Id;
        }

        RecalculateSale(existing);
        Save();
        return true;
    }

    public bool DeleteSale(int saleId)
    {
        var existing = _store.Sales.FirstOrDefault(x => x.Id == saleId);
        if (existing is null)
        {
            return false;
        }

        var removed = _store.Sales.Remove(existing);
        if (removed)
        {
            Save();
        }

        return removed;
    }

    private static AppDataStore LoadOrCreateStore(string storagePath)
    {
        if (!File.Exists(storagePath))
        {
            return new AppDataStore();
        }

        var json = File.ReadAllText(storagePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new AppDataStore();
        }

        return JsonSerializer.Deserialize<AppDataStore>(json) ?? new AppDataStore();
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_store, _jsonOptions);
        File.WriteAllText(_storagePath, json);
    }

    private void RefreshCounters()
    {
        _nextProjectId = NextId(_store.Projects.Select(x => x.Id));
        _nextContactId = NextId(_store.Contacts.Select(x => x.Id));
        _nextProductId = NextId(_store.Products.Select(x => x.Id));
        _nextPurchaseId = NextId(_store.Purchases.Select(x => x.Id));
        _nextPurchaseItemId = NextId(_store.Purchases.SelectMany(x => x.Items).Select(x => x.Id));
        _nextSaleId = NextId(_store.Sales.Select(x => x.Id));
        _nextSaleItemId = NextId(_store.Sales.SelectMany(x => x.Items).Select(x => x.Id));
    }

    private static int NextId(IEnumerable<int> ids)
    {
        return (ids.Any() ? ids.Max() : 0) + 1;
    }

    private static void SeedIfEmpty(AppDataStore store)
    {
        if (store.Projects.Count > 0 || store.Contacts.Count > 0 || store.Products.Count > 0)
        {
            return;
        }

        store.Projects.Add(
            new Project
            {
                Id = 1,
                ProjectNo = "PRJ-001",
                Name = "Govt Annual Supply",
                Description = "5,000 unit annual supply order",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 12, 31)
            });

        store.Contacts.Add(
            new Contact
            {
                Id = 1,
                ContactType = "Supplier",
                ContactCategory = "Business",
                BusinessName = "ABC Imports",
                FirstName = "Ali",
                LastName = "Khan",
                Email = "ali@abc-imports.example",
                Mobile = "+92-300-000000",
                City = "Lahore",
                Country = "Pakistan",
                VatNumber = "VAT-001",
                PaymentTermDays = 30
            });

        store.Contacts.Add(
            new Contact
            {
                Id = 2,
                ContactType = "Customer",
                ContactCategory = "Business",
                BusinessName = "Govt Procurement Wing",
                Email = "procurement@gov.example",
                Phone = "+92-51-5551234",
                City = "Islamabad",
                Country = "Pakistan",
                PaymentTermDays = 45
            });

        store.Products.Add(
            new Product
            {
                Id = 1,
                Sku = "SKU-1001",
                Name = "Medical Gloves",
                Category = "Supplies",
                Type = "Stock Product",
                Unit = "Nos",
                SalePrice = 65m,
                Cost = 52m,
                VatRate = 5m
            });

        store.Products.Add(
            new Product
            {
                Id = 2,
                Sku = "SKU-2002",
                Name = "Inspection Service",
                Category = "Service",
                Type = "Service",
                Unit = "Nos",
                SalePrice = 15000m,
                Cost = 10000m,
                VatRate = 0m
            });
    }

    private static void RecalculatePurchase(Purchase purchase)
    {
        purchase.Items ??= [];
        foreach (var item in purchase.Items)
        {
            item.Amount = item.Qty * item.Rate;
            item.VatAmount = item.Amount * (item.VatRate / 100m);
            item.IncomeTaxAmount = item.Amount * (item.IncomeTaxRate / 100m);
            item.NetAmount = item.Amount + item.CustomDutyAmount + item.VatAmount + item.IncomeTaxAmount;
        }

        purchase.TotalAmount = purchase.Items.Sum(x => x.NetAmount);
        if (purchase.Balance <= 0m)
        {
            purchase.Balance = purchase.TotalAmount;
        }
    }

    private static void RecalculateSale(Sale sale)
    {
        sale.Items ??= [];
        foreach (var item in sale.Items)
        {
            item.Amount = item.Qty * item.Rate;
            item.DiscountAmount = item.Amount * (item.DiscountPercent / 100m);
            var taxable = item.Amount - item.DiscountAmount;
            item.VatAmount = taxable * (item.VatRate / 100m);
            item.NetAmount = taxable + item.VatAmount;
        }

        sale.TotalAmount = sale.Items.Sum(x => x.NetAmount);
        if (sale.Balance <= 0m)
        {
            sale.Balance = sale.TotalAmount;
        }
    }

    private static Project CloneProject(Project item)
    {
        return new Project
        {
            Id = item.Id,
            ProjectNo = item.ProjectNo,
            Name = item.Name,
            Description = item.Description,
            StartDate = item.StartDate,
            EndDate = item.EndDate
        };
    }

    private static Contact CloneContact(Contact item)
    {
        return new Contact
        {
            Id = item.Id,
            ContactType = item.ContactType,
            ContactCategory = item.ContactCategory,
            BusinessName = item.BusinessName,
            Title = item.Title,
            FirstName = item.FirstName,
            LastName = item.LastName,
            Email = item.Email,
            Mobile = item.Mobile,
            Phone = item.Phone,
            AccountNo = item.AccountNo,
            Website = item.Website,
            Address = item.Address,
            City = item.City,
            Province = item.Province,
            PostalCode = item.PostalCode,
            Country = item.Country,
            BatakaNumber = item.BatakaNumber,
            BusinessLicenseNumber = item.BusinessLicenseNumber,
            VatNumber = item.VatNumber,
            PaymentTermDays = item.PaymentTermDays,
            CreditLimit = item.CreditLimit,
            OpeningBalance = item.OpeningBalance,
            Notes = item.Notes
        };
    }

    private static Product CloneProduct(Product item)
    {
        return new Product
        {
            Id = item.Id,
            Sku = item.Sku,
            Name = item.Name,
            Category = item.Category,
            Type = item.Type,
            StockAccount = item.StockAccount,
            LowStockLevel = item.LowStockLevel,
            SalePrice = item.SalePrice,
            Cost = item.Cost,
            SaleDiscount = item.SaleDiscount,
            PurchaseDiscount = item.PurchaseDiscount,
            Weight = item.Weight,
            Unit = item.Unit,
            VatRate = item.VatRate,
            AdtRate = item.AdtRate,
            BinLocation = item.BinLocation,
            Notes = item.Notes
        };
    }

    private static Purchase ClonePurchase(Purchase item)
    {
        return new Purchase
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            SupplierId = item.SupplierId,
            Date = item.Date,
            DueDate = item.DueDate,
            TermDays = item.TermDays,
            PoNumber = item.PoNumber,
            TotalAmount = item.TotalAmount,
            Balance = item.Balance,
            Items = item.Items.Select(ClonePurchaseItem).ToList()
        };
    }

    private static PurchaseItem ClonePurchaseItem(PurchaseItem item)
    {
        return new PurchaseItem
        {
            Id = item.Id,
            PurchaseId = item.PurchaseId,
            ProductId = item.ProductId,
            Description = item.Description,
            Qty = item.Qty,
            Unit = item.Unit,
            Rate = item.Rate,
            Amount = item.Amount,
            CustomDuty = item.CustomDuty,
            VatRate = item.VatRate,
            VatAmount = item.VatAmount,
            IncomeTaxRate = item.IncomeTaxRate,
            IncomeTaxAmount = item.IncomeTaxAmount,
            NetAmount = item.NetAmount
        };
    }

    private static Sale CloneSale(Sale item)
    {
        return new Sale
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            CustomerId = item.CustomerId,
            Date = item.Date,
            DueDate = item.DueDate,
            TermDays = item.TermDays,
            PoNumber = item.PoNumber,
            TotalAmount = item.TotalAmount,
            Balance = item.Balance,
            Items = item.Items.Select(CloneSaleItem).ToList()
        };
    }

    private static SaleItem CloneSaleItem(SaleItem item)
    {
        return new SaleItem
        {
            Id = item.Id,
            SaleId = item.SaleId,
            ProductId = item.ProductId,
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
}
