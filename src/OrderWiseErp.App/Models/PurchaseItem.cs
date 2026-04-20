namespace OrderWiseErp.App.Models;

public sealed class PurchaseItem
{
    public int Id { get; set; }

    public int PurchaseId { get; set; }

    public int ProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Qty { get; set; }

    public string Unit { get; set; } = "Nos";

    public decimal Rate { get; set; }

    public decimal Amount { get; set; }

    public decimal CustomDuty { get; set; }

    public decimal CustomDutyAmount => Amount * (CustomDuty / 100m);

    public decimal VatRate { get; set; }

    public decimal VatAmount { get; set; }

    public decimal IncomeTaxRate { get; set; }

    public decimal IncomeTaxAmount { get; set; }

    public decimal NetAmount { get; set; }
}
