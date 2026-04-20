using OrderWiseErp.App.Models;

namespace OrderWiseErp.App.Services;

public sealed class InMemoryAppDataService : IAppDataService
{
    private readonly List<Project> _projects;
    private readonly List<Contact> _contacts;
    private readonly List<Product> _products;

    private int _nextProjectId = 1;
    private int _nextContactId = 1;
    private int _nextProductId = 1;

    public InMemoryAppDataService()
    {
        _projects =
        [
            new Project
            {
                Id = _nextProjectId++,
                ProjectNo = "PRJ-001",
                Name = "Govt Annual Supply",
                Description = "5,000 unit annual supply order",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 12, 31)
            }
        ];

        _contacts =
        [
            new Contact
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
            new Contact
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
        ];

        _products =
        [
            new Product
            {
                Id = _nextProductId++,
                Sku = "SKU-1001",
                Name = "Medical Gloves",
                Category = "Supplies",
                SalePrice = 65m,
                Cost = 52m,
                VatRate = 5m
            },
            new Product
            {
                Id = _nextProductId++,
                Sku = "SKU-2002",
                Name = "Inspection Service",
                Category = "Service",
                SalePrice = 15000m,
                Cost = 10000m,
                VatRate = 0m
            }
        ];
    }

    public IReadOnlyList<Project> Projects => _projects;

    public IReadOnlyList<Contact> Contacts => _contacts;

    public IReadOnlyList<Product> Products => _products;

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
        return true;
    }

    public bool DeleteProject(int projectId)
    {
        var existing = _projects.FirstOrDefault(x => x.Id == projectId);
        return existing is not null && _projects.Remove(existing);
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
        existing.VatNumber = contact.VatNumber;
        existing.PaymentTermDays = contact.PaymentTermDays;
        existing.CreditLimit = contact.CreditLimit;
        existing.Notes = contact.Notes;
        return true;
    }

    public bool DeleteContact(int contactId)
    {
        var existing = _contacts.FirstOrDefault(x => x.Id == contactId);
        return existing is not null && _contacts.Remove(existing);
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
        return true;
    }

    public bool DeleteProduct(int productId)
    {
        var existing = _products.FirstOrDefault(x => x.Id == productId);
        return existing is not null && _products.Remove(existing);
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
            VatNumber = item.VatNumber,
            PaymentTermDays = item.PaymentTermDays,
            CreditLimit = item.CreditLimit,
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
}
