namespace OrderWiseErp.App.Models;

public sealed class Product
{
    public int Id { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Type { get; set; } = "Stock Product";

    public string StockAccount { get; set; } = string.Empty;

    public int LowStockLevel { get; set; }

    public decimal SalePrice { get; set; }

    public decimal Cost { get; set; }

    public decimal SaleDiscount { get; set; }

    public decimal PurchaseDiscount { get; set; }

    public decimal Weight { get; set; }

    public string Unit { get; set; } = "Nos";

    public decimal VatRate { get; set; }

    public decimal AdtRate { get; set; }

    public string BinLocation { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
}
