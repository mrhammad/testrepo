namespace OrderWiseErp.App.Models;

public sealed class SaleItem
{
    public int Id { get; set; }

    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Qty { get; set; }

    public string Unit { get; set; } = "Nos";

    public decimal Rate { get; set; }

    public decimal Amount { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal VatRate { get; set; }

    public decimal VatAmount { get; set; }

    public decimal NetAmount { get; set; }
}
