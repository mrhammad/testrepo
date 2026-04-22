namespace OrderWiseErp.App.Models;

public sealed class AppDataStore
{
    public List<Project> Projects { get; set; } = [];

    public List<Contact> Contacts { get; set; } = [];

    public List<Product> Products { get; set; } = [];

    public List<Purchase> Purchases { get; set; } = [];

    public List<Sale> Sales { get; set; } = [];

    public List<StockAdjustment> StockAdjustments { get; set; } = [];

    public List<StockTransfer> StockTransfers { get; set; } = [];
}
