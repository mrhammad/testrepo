using System.IO;
using System.Text.Json;
using OrderWiseErp.App.Models;

namespace OrderWiseErp.App.Services;

public sealed class InMemoryAppDataService : IAppDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly List<Project> _projects;
    private readonly List<Contact> _contacts;
    private readonly List<Product> _products;
    private readonly List<Purchase> _purchases;
    private readonly List<Sale> _sales;
    private readonly List<StockAdjustment> _stockAdjustments;
    private readonly List<StockTransfer> _stockTransfers;
    private readonly string _dataFilePath;

    private int _nextProjectId = 1;
    private int _nextContactId = 1;
    private int _nextProductId = 1;
    private int _nextPurchaseId = 1;
    private int _nextPurchaseItemId = 1;
    private int _nextSaleId = 1;
    private int _nextSaleItemId = 1;
    private int _nextStockAdjustmentId = 1;
    private int _nextStockAdjustmentItemId = 1;
    private int _nextStockTransferId = 1;
    private int _nextStockTransferItemId = 1;

    public InMemoryAppDataService()
    {
        var appDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OrderWiseErp");
        Directory.CreateDirectory(appDataDirectory);
        _dataFilePath = Path.Combine(appDataDirectory, "app-data.json");

        var existingData = LoadStore();
        if (existingData is null)
        {
            (_projects, _contacts, _products, _purchases, _sales, _stockAdjustments, _stockTransfers) = BuildSeedData();
            SaveStore();
        }
        else
        {
            _projects = existingData.Projects ?? [];
            _contacts = existingData.Contacts ?? [];
            _products = existingData.Products ?? [];
            _purchases = existingData.Purchases ?? [];
            _sales = existingData.Sales ?? [];
            _stockAdjustments = existingData.StockAdjustments ?? [];
            _stockTransfers = existingData.StockTransfers ?? [];
            EnsureItemIds();
        }

        RefreshNextIds();
    }

    public IReadOnlyList<Project> Projects => _projects;

    public IReadOnlyList<Contact> Contacts => _contacts;

    public IReadOnlyList<Product> Products => _products;

    public IReadOnlyList<Purchase> Purchases => _purchases;

    public IReadOnlyList<Sale> Sales => _sales;

    public IReadOnlyList<StockAdjustment> StockAdjustments => _stockAdjustments;

    public IReadOnlyList<StockTransfer> StockTransfers => _stockTransfers;

    public IReadOnlyList<Project> GetProjects()
    {
        return _projects
            .OrderBy(x => x.ProjectNo)
            .Select(CloneProject)
            .ToList();
    }

    public Project AddProject(Project project)
    {
        var copy = CloneProject(project);
        copy.Id = _nextProjectId++;
        _projects.Add(copy);
        SaveStore();
        return CloneProject(copy);
    }

    public bool UpdateProject(Project project)
    {
        var existing = _projects.FirstOrDefault(x => x.Id == project.Id);
        if (existing is null)
        {
            return false;
        }

        existing.ProjectNo = project.ProjectNo;
        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.StartDate = project.StartDate;
        existing.EndDate = project.EndDate;
        SaveStore();
        return true;
    }

    public bool DeleteProject(int projectId)
    {
        var existing = _projects.FirstOrDefault(x => x.Id == projectId);
        if (existing is null)
        {
            return false;
        }

        var removed = _projects.Remove(existing);
        if (removed)
        {
            SaveStore();
        }

        return removed;
    }

    public IReadOnlyList<Contact> GetContacts()
    {
        return _contacts
            .OrderBy(x => x.BusinessName)
            .Select(CloneContact)
            .ToList();
    }

    public Contact AddContact(Contact contact)
    {
        var copy = CloneContact(contact);
        copy.Id = _nextContactId++;
        _contacts.Add(copy);
        SaveStore();
        return CloneContact(copy);
    }

    public bool UpdateContact(Contact contact)
    {
        var existing = _contacts.FirstOrDefault(x => x.Id == contact.Id);
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
        SaveStore();
        return true;
    }

    public bool DeleteContact(int contactId)
    {
        var existing = _contacts.FirstOrDefault(x => x.Id == contactId);
        if (existing is null)
        {
            return false;
        }

        var removed = _contacts.Remove(existing);
        if (removed)
        {
            SaveStore();
        }

        return removed;
    }

    public IReadOnlyList<Product> GetProducts()
    {
        return _products
            .OrderBy(x => x.Sku)
            .Select(CloneProduct)
            .ToList();
    }

    public Product AddProduct(Product product)
    {
        var copy = CloneProduct(product);
        copy.Id = _nextProductId++;
        _products.Add(copy);
        SaveStore();
        return CloneProduct(copy);
    }

    public bool UpdateProduct(Product product)
    {
        var existing = _products.FirstOrDefault(x => x.Id == product.Id);
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
        SaveStore();
        return true;
    }

    public bool DeleteProduct(int productId)
    {
        var existing = _products.FirstOrDefault(x => x.Id == productId);
        if (existing is null)
        {
            return false;
        }

        var removed = _products.Remove(existing);
        if (removed)
        {
            SaveStore();
        }

        return removed;
    }

    public IReadOnlyList<Purchase> GetPurchases()
    {
        return _purchases
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Select(ClonePurchase)
            .ToList();
    }

    public Purchase AddPurchase(Purchase purchase)
    {
        var copy = ClonePurchase(purchase);
        copy.Id = _nextPurchaseId++;
        AssignPurchaseItemIds(copy);
        RecalculatePurchase(copy);
        _purchases.Add(copy);
        SaveStore();
        return ClonePurchase(copy);
    }

    public bool UpdatePurchase(Purchase purchase)
    {
        var existing = _purchases.FirstOrDefault(x => x.Id == purchase.Id);
        if (existing is null)
        {
            return false;
        }

        var copy = ClonePurchase(purchase);
        copy.Id = existing.Id;
        AssignPurchaseItemIds(copy);
        RecalculatePurchase(copy);
        var index = _purchases.IndexOf(existing);
        _purchases[index] = copy;
        SaveStore();
        return true;
    }

    public bool DeletePurchase(int purchaseId)
    {
        var existing = _purchases.FirstOrDefault(x => x.Id == purchaseId);
        if (existing is null)
        {
            return false;
        }

        var removed = _purchases.Remove(existing);
        if (removed)
        {
            SaveStore();
        }

        return removed;
    }

    public IReadOnlyList<Sale> GetSales()
    {
        return _sales
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Select(CloneSale)
            .ToList();
    }

    public Sale AddSale(Sale sale)
    {
        var copy = CloneSale(sale);
        copy.Id = _nextSaleId++;
        AssignSaleItemIds(copy);
        RecalculateSale(copy);
        _sales.Add(copy);
        SaveStore();
        return CloneSale(copy);
    }

    public bool UpdateSale(Sale sale)
    {
        var existing = _sales.FirstOrDefault(x => x.Id == sale.Id);
        if (existing is null)
        {
            return false;
        }

        var copy = CloneSale(sale);
        copy.Id = existing.Id;
        AssignSaleItemIds(copy);
        RecalculateSale(copy);
        var index = _sales.IndexOf(existing);
        _sales[index] = copy;
        SaveStore();
        return true;
    }

    public bool DeleteSale(int saleId)
    {
        var existing = _sales.FirstOrDefault(x => x.Id == saleId);
        if (existing is null)
        {
            return false;
        }

        var removed = _sales.Remove(existing);
        if (removed)
        {
            SaveStore();
        }

        return removed;
    }

    public IReadOnlyList<StockAdjustment> GetStockAdjustments()
    {
        return _stockAdjustments
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Select(CloneStockAdjustment)
            .ToList();
    }

    public StockAdjustment AddStockAdjustment(StockAdjustment adjustment)
    {
        var copy = CloneStockAdjustment(adjustment);
        copy.Id = _nextStockAdjustmentId++;
        AssignStockAdjustmentItemIds(copy);
        RecalculateStockAdjustment(copy);
        _stockAdjustments.Add(copy);
        SaveStore();
        return CloneStockAdjustment(copy);
    }

    public bool UpdateStockAdjustment(StockAdjustment adjustment)
    {
        var existing = _stockAdjustments.FirstOrDefault(x => x.Id == adjustment.Id);
        if (existing is null)
        {
            return false;
        }

        var copy = CloneStockAdjustment(adjustment);
        copy.Id = existing.Id;
        AssignStockAdjustmentItemIds(copy);
        RecalculateStockAdjustment(copy);
        var index = _stockAdjustments.IndexOf(existing);
        _stockAdjustments[index] = copy;
        SaveStore();
        return true;
    }

    public bool DeleteStockAdjustment(int adjustmentId)
    {
        var existing = _stockAdjustments.FirstOrDefault(x => x.Id == adjustmentId);
        if (existing is null)
        {
            return false;
        }

        var removed = _stockAdjustments.Remove(existing);
        if (removed)
        {
            SaveStore();
        }

        return removed;
    }

    public IReadOnlyList<StockTransfer> GetStockTransfers()
    {
        return _stockTransfers
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Select(CloneStockTransfer)
            .ToList();
    }

    public StockTransfer AddStockTransfer(StockTransfer transfer)
    {
        var copy = CloneStockTransfer(transfer);
        copy.Id = _nextStockTransferId++;
        AssignStockTransferItemIds(copy);
        RecalculateStockTransfer(copy);
        _stockTransfers.Add(copy);
        SaveStore();
        return CloneStockTransfer(copy);
    }

    public bool UpdateStockTransfer(StockTransfer transfer)
    {
        var existing = _stockTransfers.FirstOrDefault(x => x.Id == transfer.Id);
        if (existing is null)
        {
            return false;
        }

        var copy = CloneStockTransfer(transfer);
        copy.Id = existing.Id;
        AssignStockTransferItemIds(copy);
        RecalculateStockTransfer(copy);
        var index = _stockTransfers.IndexOf(existing);
        _stockTransfers[index] = copy;
        SaveStore();
        return true;
    }

    public bool DeleteStockTransfer(int transferId)
    {
        var existing = _stockTransfers.FirstOrDefault(x => x.Id == transferId);
        if (existing is null)
        {
            return false;
        }

        var removed = _stockTransfers.Remove(existing);
        if (removed)
        {
            SaveStore();
        }

        return removed;
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
            Date = item.Date,
            ReferenceNo = item.ReferenceNo,
            CustomerOrSupplier = item.CustomerOrSupplier,
            Location = item.Location,
            AdjustmentType = item.AdjustmentType,
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
            Date = item.Date,
            ReferenceNo = item.ReferenceNo,
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

    private void SaveStore()
    {
        var store = new AppDataStore
        {
            Projects = _projects,
            Contacts = _contacts,
            Products = _products,
            Purchases = _purchases,
            Sales = _sales,
            StockAdjustments = _stockAdjustments,
            StockTransfers = _stockTransfers
        };

        var json = JsonSerializer.Serialize(store, JsonOptions);
        File.WriteAllText(_dataFilePath, json);
    }

    private AppDataStore? LoadStore()
    {
        if (!File.Exists(_dataFilePath))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<AppDataStore>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private (List<Project>, List<Contact>, List<Product>, List<Purchase>, List<Sale>, List<StockAdjustment>, List<StockTransfer>) BuildSeedData()
    {
        var projects = new List<Project>
        {
            new()
            {
                Id = _nextProjectId++,
                ProjectNo = "PRJ-001",
                Name = "Govt Annual Supply",
                Description = "5,000 unit annual supply order",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 12, 31)
            }
        };

        var contacts = new List<Contact>
        {
            new()
            {
                Id = _nextContactId++,
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
            },
            new()
            {
                Id = _nextContactId++,
                ContactType = "Customer",
                ContactCategory = "Business",
                BusinessName = "Govt Procurement Wing",
                Email = "procurement@gov.example",
                Phone = "+92-51-5551234",
                City = "Islamabad",
                Country = "Pakistan",
                PaymentTermDays = 45
            }
        };

        var products = new List<Product>
        {
            new()
            {
                Id = _nextProductId++,
                Sku = "SKU-1001",
                Name = "Medical Gloves",
                Category = "Supplies",
                SalePrice = 65m,
                Cost = 52m,
                VatRate = 5m
            },
            new()
            {
                Id = _nextProductId++,
                Sku = "SKU-2002",
                Name = "Inspection Service",
                Category = "Service",
                SalePrice = 15000m,
                Cost = 10000m,
                VatRate = 0m
            }
        };

        var purchases = new List<Purchase>();
        var sales = new List<Sale>();
        if (projects.Count > 0 && contacts.Count > 1 && products.Count > 0)
        {
            var seedPurchase = new Purchase
            {
                Id = _nextPurchaseId++,
                PurchaseNo = "PUR-0001",
                ProjectId = projects[0].Id,
                ProjectNo = projects[0].ProjectNo,
                SupplierId = contacts[0].Id,
                SupplierName = contacts[0].BusinessName,
                Date = new DateTime(2026, 1, 15),
                DueDate = new DateTime(2026, 2, 14),
                TermDays = 30,
                PoNumber = "PO-1001"
            };

            seedPurchase.Items.Add(
                new PurchaseItem
                {
                    Id = _nextPurchaseItemId++,
                    PurchaseId = seedPurchase.Id,
                    ProductId = products[0].Id,
                    Sku = products[0].Sku,
                    ProductName = products[0].Name,
                    Description = "Initial stock purchase",
                    Qty = 500m,
                    Unit = products[0].Unit,
                    Rate = products[0].Cost,
                    CustomDuty = 5m,
                    VatRate = 5m,
                    IncomeTaxRate = 2m
                });
            RecalculatePurchase(seedPurchase);
            purchases.Add(seedPurchase);

            var seedSale = new Sale
            {
                Id = _nextSaleId++,
                InvoiceNo = "SAL-0001",
                ProjectId = projects[0].Id,
                ProjectNo = projects[0].ProjectNo,
                CustomerId = contacts[1].Id,
                CustomerName = contacts[1].BusinessName,
                Date = new DateTime(2026, 1, 28),
                DueDate = new DateTime(2026, 3, 14),
                TermDays = 45,
                PoNumber = "SO-2001"
            };

            seedSale.Items.Add(
                new SaleItem
                {
                    Id = _nextSaleItemId++,
                    SaleId = seedSale.Id,
                    ProductId = products[0].Id,
                    Sku = products[0].Sku,
                    ProductName = products[0].Name,
                    Description = "Initial project delivery",
                    Qty = 200m,
                    Unit = products[0].Unit,
                    Rate = products[0].SalePrice,
                    DiscountPercent = 1.5m,
                    VatRate = 5m
                });
            RecalculateSale(seedSale);
            sales.Add(seedSale);
        }
        var stockAdjustments = new List<StockAdjustment>
        {
            new()
            {
                Id = _nextStockAdjustmentId++,
                Date = new DateTime(2026, 2, 5),
                ReferenceNo = "SA-0001",
                CustomerOrSupplier = "ABC Imports",
                Location = "Main Warehouse",
                AdjustmentType = "Increase",
                TotalAmountRecovered = 0m,
                Reason = "Opening stock correction",
                AddedBy = "System",
                Items =
                [
                    new StockAdjustmentItem
                    {
                        Id = _nextStockAdjustmentItemId++,
                        ProductId = products[0].Id,
                        Sku = products[0].Sku,
                        ProductName = products[0].Name,
                        Qty = 10m,
                        UnitPrice = products[0].Cost
                    }
                ]
            }
        };
        RecalculateStockAdjustment(stockAdjustments[0]);

        var stockTransfers = new List<StockTransfer>
        {
            new()
            {
                Id = _nextStockTransferId++,
                Date = new DateTime(2026, 2, 10),
                ReferenceNo = "ST-0001",
                LocationFrom = "Main Warehouse",
                LocationTo = "Gov Dispatch Hub",
                Status = "Completed",
                CustomerOrSupplier = "Govt Procurement Wing",
                ShippingCharges = 200m,
                AdditionalNote = "Initial deployment",
                Items =
                [
                    new StockTransferItem
                    {
                        Id = _nextStockTransferItemId++,
                        ProductId = products[0].Id,
                        Sku = products[0].Sku,
                        ProductName = products[0].Name,
                        Qty = 25m,
                        UnitPrice = products[0].Cost
                    }
                ]
            }
        };
        RecalculateStockTransfer(stockTransfers[0]);

        return (projects, contacts, products, purchases, sales, stockAdjustments, stockTransfers);
    }

    private void RefreshNextIds()
    {
        _nextProjectId = _projects.Count == 0 ? 1 : _projects.Max(x => x.Id) + 1;
        _nextContactId = _contacts.Count == 0 ? 1 : _contacts.Max(x => x.Id) + 1;
        _nextProductId = _products.Count == 0 ? 1 : _products.Max(x => x.Id) + 1;
        _nextPurchaseId = _purchases.Count == 0 ? 1 : _purchases.Max(x => x.Id) + 1;
        _nextPurchaseItemId = _purchases.SelectMany(x => x.Items).DefaultIfEmpty(new PurchaseItem { Id = 0 }).Max(x => x.Id) + 1;
        _nextSaleId = _sales.Count == 0 ? 1 : _sales.Max(x => x.Id) + 1;
        _nextSaleItemId = _sales.SelectMany(x => x.Items).DefaultIfEmpty(new SaleItem { Id = 0 }).Max(x => x.Id) + 1;
        _nextStockAdjustmentId = _stockAdjustments.Count == 0 ? 1 : _stockAdjustments.Max(x => x.Id) + 1;
        _nextStockAdjustmentItemId = _stockAdjustments.SelectMany(x => x.Items).DefaultIfEmpty(new StockAdjustmentItem { Id = 0 }).Max(x => x.Id) + 1;
        _nextStockTransferId = _stockTransfers.Count == 0 ? 1 : _stockTransfers.Max(x => x.Id) + 1;
        _nextStockTransferItemId = _stockTransfers.SelectMany(x => x.Items).DefaultIfEmpty(new StockTransferItem { Id = 0 }).Max(x => x.Id) + 1;
    }

    private void EnsureItemIds()
    {
        foreach (var purchase in _purchases)
        {
            foreach (var item in purchase.Items)
            {
                if (item.Id <= 0)
                {
                    item.Id = _nextPurchaseItemId++;
                }
            }

            RecalculatePurchase(purchase);
        }

        foreach (var sale in _sales)
        {
            foreach (var item in sale.Items)
            {
                if (item.Id <= 0)
                {
                    item.Id = _nextSaleItemId++;
                }
            }

            RecalculateSale(sale);
        }

        foreach (var adjustment in _stockAdjustments)
        {
            foreach (var item in adjustment.Items)
            {
                if (item.Id <= 0)
                {
                    item.Id = _nextStockAdjustmentItemId++;
                }
            }

            RecalculateStockAdjustment(adjustment);
        }

        foreach (var transfer in _stockTransfers)
        {
            foreach (var item in transfer.Items)
            {
                if (item.Id <= 0)
                {
                    item.Id = _nextStockTransferItemId++;
                }
            }

            RecalculateStockTransfer(transfer);
        }
    }

    private void AssignPurchaseItemIds(Purchase purchase)
    {
        foreach (var item in purchase.Items)
        {
            if (item.Id <= 0)
            {
                item.Id = _nextPurchaseItemId++;
            }
        }
    }

    private void AssignSaleItemIds(Sale sale)
    {
        foreach (var item in sale.Items)
        {
            if (item.Id <= 0)
            {
                item.Id = _nextSaleItemId++;
            }
        }
    }

    private void AssignStockAdjustmentItemIds(StockAdjustment adjustment)
    {
        foreach (var item in adjustment.Items)
        {
            if (item.Id <= 0)
            {
                item.Id = _nextStockAdjustmentItemId++;
            }
        }
    }

    private void AssignStockTransferItemIds(StockTransfer transfer)
    {
        foreach (var item in transfer.Items)
        {
            if (item.Id <= 0)
            {
                item.Id = _nextStockTransferItemId++;
            }
        }
    }

    private static void RecalculatePurchase(Purchase purchase)
    {
        foreach (var item in purchase.Items)
        {
            item.Amount = item.Qty * item.Rate;
            item.VatAmount = item.Amount * item.VatRate / 100m;
            item.IncomeTaxAmount = item.Amount * item.IncomeTaxRate / 100m;
            item.NetAmount = item.Amount + item.CustomDutyAmount + item.VatAmount + item.IncomeTaxAmount;
        }

        purchase.TotalAmount = purchase.Items.Sum(x => x.NetAmount);
        purchase.Balance = purchase.TotalAmount;
    }

    private static void RecalculateSale(Sale sale)
    {
        foreach (var item in sale.Items)
        {
            item.Amount = item.Qty * item.Rate;
            item.DiscountAmount = item.Amount * item.DiscountPercent / 100m;
            var taxable = item.Amount - item.DiscountAmount;
            item.VatAmount = taxable * item.VatRate / 100m;
            item.NetAmount = taxable + item.VatAmount;
        }

        sale.TotalAmount = sale.Items.Sum(x => x.NetAmount);
        sale.Balance = sale.TotalAmount;
    }

    private static void RecalculateStockAdjustment(StockAdjustment adjustment)
    {
        foreach (var item in adjustment.Items)
        {
            item.SubTotal = item.Qty * item.UnitPrice;
        }

        adjustment.TotalAmount = adjustment.Items.Sum(x => x.SubTotal);
    }

    private static void RecalculateStockTransfer(StockTransfer transfer)
    {
        foreach (var item in transfer.Items)
        {
            item.SubTotal = item.Qty * item.UnitPrice;
        }

        transfer.TotalAmount = transfer.Items.Sum(x => x.SubTotal) + transfer.ShippingCharges;
    }
}
