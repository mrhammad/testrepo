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
    private int _nextStockAdjustmentId;
    private int _nextStockAdjustmentItemId;
    private int _nextStockTransferId;
    private int _nextStockTransferItemId;

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

    public IReadOnlyList<StockAdjustment> StockAdjustments => _store.StockAdjustments;

    public IReadOnlyList<StockTransfer> StockTransfers => _store.StockTransfers;

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

    public IReadOnlyList<StockAdjustment> GetStockAdjustments()
    {
        return _store.StockAdjustments
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Select(CloneStockAdjustment)
            .ToList();
    }

    public IReadOnlyList<StockTransfer> GetStockTransfers()
    {
        return _store.StockTransfers
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Select(CloneStockTransfer)
            .ToList();
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

        existing.PurchaseNo = purchase.PurchaseNo;
        existing.ProjectNo = purchase.ProjectNo;
        existing.ProjectId = purchase.ProjectId;
        existing.SupplierName = purchase.SupplierName;
        existing.SupplierId = purchase.SupplierId;
        existing.Date = purchase.Date;
        existing.DueDate = purchase.DueDate;
        existing.TermDays = purchase.TermDays;
        existing.PoNumber = purchase.PoNumber;
        existing.CreatedBy = purchase.CreatedBy;
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

        existing.InvoiceNo = sale.InvoiceNo;
        existing.ProjectNo = sale.ProjectNo;
        existing.ProjectId = sale.ProjectId;
        existing.CustomerName = sale.CustomerName;
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

    public StockAdjustment AddStockAdjustment(StockAdjustment adjustment)
    {
        var copy = CloneStockAdjustment(adjustment);
        copy.Id = _nextStockAdjustmentId++;
        copy.Items = copy.Items.Select(CloneStockAdjustmentItem).ToList();
        foreach (var item in copy.Items)
        {
            item.Id = _nextStockAdjustmentItemId++;
            item.StockAdjustmentId = copy.Id;
        }

        RecalculateStockAdjustment(copy);
        _store.StockAdjustments.Add(copy);
        Save();
        return CloneStockAdjustment(copy);
    }

    public bool UpdateStockAdjustment(StockAdjustment adjustment)
    {
        var existing = _store.StockAdjustments.FirstOrDefault(x => x.Id == adjustment.Id);
        if (existing is null)
        {
            return false;
        }

        existing.ReferenceNo = adjustment.ReferenceNo;
        existing.Date = adjustment.Date;
        existing.Location = adjustment.Location;
        existing.AdjustmentType = adjustment.AdjustmentType;
        existing.CustomerOrSupplier = adjustment.CustomerOrSupplier;
        existing.TotalAmountRecovered = adjustment.TotalAmountRecovered;
        existing.Reason = adjustment.Reason;
        existing.AddedBy = adjustment.AddedBy;
        existing.Items = adjustment.Items.Select(CloneStockAdjustmentItem).ToList();

        foreach (var item in existing.Items)
        {
            if (item.Id <= 0)
            {
                item.Id = _nextStockAdjustmentItemId++;
            }

            item.StockAdjustmentId = existing.Id;
        }

        RecalculateStockAdjustment(existing);
        Save();
        return true;
    }

    public bool DeleteStockAdjustment(int stockAdjustmentId)
    {
        var existing = _store.StockAdjustments.FirstOrDefault(x => x.Id == stockAdjustmentId);
        if (existing is null)
        {
            return false;
        }

        var removed = _store.StockAdjustments.Remove(existing);
        if (removed)
        {
            Save();
        }

        return removed;
    }

    public StockTransfer AddStockTransfer(StockTransfer transfer)
    {
        var copy = CloneStockTransfer(transfer);
        copy.Id = _nextStockTransferId++;
        copy.Items = copy.Items.Select(CloneStockTransferItem).ToList();
        foreach (var item in copy.Items)
        {
            item.Id = _nextStockTransferItemId++;
            item.StockTransferId = copy.Id;
        }

        RecalculateStockTransfer(copy);
        _store.StockTransfers.Add(copy);
        Save();
        return CloneStockTransfer(copy);
    }

    public bool UpdateStockTransfer(StockTransfer transfer)
    {
        var existing = _store.StockTransfers.FirstOrDefault(x => x.Id == transfer.Id);
        if (existing is null)
        {
            return false;
        }

        existing.ReferenceNo = transfer.ReferenceNo;
        existing.Date = transfer.Date;
        existing.LocationFrom = transfer.LocationFrom;
        existing.LocationTo = transfer.LocationTo;
        existing.Status = transfer.Status;
        existing.CustomerOrSupplier = transfer.CustomerOrSupplier;
        existing.ShippingCharges = transfer.ShippingCharges;
        existing.AdditionalNote = transfer.AdditionalNote;
        existing.Items = transfer.Items.Select(CloneStockTransferItem).ToList();

        foreach (var item in existing.Items)
        {
            if (item.Id <= 0)
            {
                item.Id = _nextStockTransferItemId++;
            }

            item.StockTransferId = existing.Id;
        }

        RecalculateStockTransfer(existing);
        Save();
        return true;
    }

    public bool DeleteStockTransfer(int stockTransferId)
    {
        var existing = _store.StockTransfers.FirstOrDefault(x => x.Id == stockTransferId);
        if (existing is null)
        {
            return false;
        }

        var removed = _store.StockTransfers.Remove(existing);
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
        _nextStockAdjustmentId = NextId(_store.StockAdjustments.Select(x => x.Id));
        _nextStockAdjustmentItemId = NextId(_store.StockAdjustments.SelectMany(x => x.Items).Select(x => x.Id));
        _nextStockTransferId = NextId(_store.StockTransfers.Select(x => x.Id));
        _nextStockTransferItemId = NextId(_store.StockTransfers.SelectMany(x => x.Items).Select(x => x.Id));
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

        store.StockAdjustments.Add(
            new StockAdjustment
            {
                Id = 1,
                ReferenceNo = "ADJ-0001",
                Date = new DateTime(2026, 2, 10),
                Location = "Main Warehouse",
                AdjustmentType = "Increase",
                CustomerOrSupplier = "ABC Imports",
                Reason = "Opening balance correction",
                AddedBy = "System",
                TotalAmountRecovered = 0m,
                Items =
                [
                    new StockAdjustmentItem
                    {
                        Id = 1,
                        StockAdjustmentId = 1,
                        ProductId = 1,
                        Sku = "SKU-1001",
                        ProductName = "Medical Gloves",
                        Qty = 25m,
                        UnitPrice = 52m
                    }
                ]
            });
        RecalculateStockAdjustment(store.StockAdjustments[0]);

        store.StockTransfers.Add(
            new StockTransfer
            {
                Id = 1,
                ReferenceNo = "TRN-0001",
                Date = new DateTime(2026, 3, 2),
                LocationFrom = "Main Warehouse",
                LocationTo = "Project Site Store",
                Status = "Pending",
                CustomerOrSupplier = "Govt Procurement Wing",
                ShippingCharges = 150m,
                AdditionalNote = "Initial deployment",
                Items =
                [
                    new StockTransferItem
                    {
                        Id = 1,
                        StockTransferId = 1,
                        ProductId = 1,
                        Sku = "SKU-1001",
                        ProductName = "Medical Gloves",
                        Qty = 10m,
                        UnitPrice = 52m
                    }
                ]
            });
        RecalculateStockTransfer(store.StockTransfers[0]);
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

    private static void RecalculateStockAdjustment(StockAdjustment adjustment)
    {
        adjustment.Items ??= [];
        foreach (var item in adjustment.Items)
        {
            item.StockAdjustmentId = adjustment.Id;
            item.SubTotal = item.Qty * item.UnitPrice;
        }

        adjustment.TotalAmount = adjustment.Items.Sum(x => x.SubTotal);
    }

    private static void RecalculateStockTransfer(StockTransfer transfer)
    {
        transfer.Items ??= [];
        foreach (var item in transfer.Items)
        {
            item.StockTransferId = transfer.Id;
            item.SubTotal = item.Qty * item.UnitPrice;
        }

        transfer.TotalAmount = transfer.Items.Sum(x => x.SubTotal) + transfer.ShippingCharges;
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
            PurchaseNo = item.PurchaseNo,
            ProjectNo = item.ProjectNo,
            ProjectId = item.ProjectId,
            SupplierName = item.SupplierName,
            SupplierId = item.SupplierId,
            Date = item.Date,
            DueDate = item.DueDate,
            TermDays = item.TermDays,
            PoNumber = item.PoNumber,
            TotalAmount = item.TotalAmount,
            Balance = item.Balance,
            CreatedBy = item.CreatedBy,
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
            Sku = item.Sku,
            ProductName = item.ProductName,
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
            InvoiceNo = item.InvoiceNo,
            ProjectNo = item.ProjectNo,
            ProjectId = item.ProjectId,
            CustomerName = item.CustomerName,
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

    private static StockAdjustment CloneStockAdjustment(StockAdjustment item)
    {
        return new StockAdjustment
        {
            Id = item.Id,
            ReferenceNo = item.ReferenceNo,
            Date = item.Date,
            Location = item.Location,
            AdjustmentType = item.AdjustmentType,
            CustomerOrSupplier = item.CustomerOrSupplier,
            TotalAmount = item.TotalAmount,
            TotalAmountRecovered = item.TotalAmountRecovered,
            Reason = item.Reason,
            AddedBy = item.AddedBy,
            Items = item.Items.Select(CloneStockAdjustmentItem).ToList()
        };
    }

    private static StockAdjustmentItem CloneStockAdjustmentItem(StockAdjustmentItem item)
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

    private static StockTransfer CloneStockTransfer(StockTransfer item)
    {
        return new StockTransfer
        {
            Id = item.Id,
            ReferenceNo = item.ReferenceNo,
            Date = item.Date,
            LocationFrom = item.LocationFrom,
            LocationTo = item.LocationTo,
            Status = item.Status,
            CustomerOrSupplier = item.CustomerOrSupplier,
            ShippingCharges = item.ShippingCharges,
            TotalAmount = item.TotalAmount,
            AdditionalNote = item.AdditionalNote,
            Items = item.Items.Select(CloneStockTransferItem).ToList()
        };
    }

    private static StockTransferItem CloneStockTransferItem(StockTransferItem item)
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
}
