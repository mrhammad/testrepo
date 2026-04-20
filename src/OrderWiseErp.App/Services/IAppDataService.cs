using OrderWiseErp.App.Models;

namespace OrderWiseErp.App.Services;

public interface IAppDataService
{
    IReadOnlyList<Project> Projects { get; }

    IReadOnlyList<Contact> Contacts { get; }

    IReadOnlyList<Product> Products { get; }

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
}
