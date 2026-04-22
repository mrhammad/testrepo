namespace OrderWiseErp.App.Models;

public sealed class StockTransfer
{
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public string ReferenceNo { get; set; } = string.Empty;

    public string LocationFrom { get; set; } = string.Empty;

    public string LocationTo { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public string CustomerOrSupplier { get; set; } = string.Empty;

    public decimal ShippingCharges { get; set; }

    public decimal TotalAmount { get; set; }

    public string AdditionalNote { get; set; } = string.Empty;

    public List<StockTransferItem> Items { get; set; } = [];

    public void RecalculateTotals()
    {
        foreach (var item in Items)
        {
            item.SubTotal = item.Qty * item.UnitPrice;
        }

        TotalAmount = Items.Sum(x => x.SubTotal) + ShippingCharges;
    }
}

public sealed class StockTransferItem
{
    public int Id { get; set; }

    public int StockTransferId { get; set; }

    public int ProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Qty { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal SubTotal { get; set; }
}
