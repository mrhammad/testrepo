using OrderWiseErp.App.Models;

namespace OrderWiseErp.App.Services;

public interface IAppDataService
{
    IReadOnlyList<Project> Projects { get; }

    IReadOnlyList<Contact> Contacts { get; }

    IReadOnlyList<Product> Products { get; }

    IReadOnlyList<Purchase> Purchases { get; }

    IReadOnlyList<Sale> Sales { get; }

    IReadOnlyList<Project> GetProjects();
    IReadOnlyList<Contact> GetContacts();
    IReadOnlyList<Product> GetProducts();

    Project AddProject(Project project);
    bool UpdateProject(Project project);
    bool DeleteProject(int projectId);

    Contact AddContact(Contact contact);
    bool UpdateContact(Contact contact);
    bool DeleteContact(int contactId);

    Product AddProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(int productId);

    IReadOnlyList<Purchase> GetPurchases();
    Purchase AddPurchase(Purchase purchase);
    bool UpdatePurchase(Purchase purchase);
    bool DeletePurchase(int purchaseId);

    IReadOnlyList<Sale> GetSales();
    Sale AddSale(Sale sale);
    bool UpdateSale(Sale sale);
    bool DeleteSale(int saleId);
}
