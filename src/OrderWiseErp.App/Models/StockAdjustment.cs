namespace OrderWiseErp.App.Models;

public sealed class StockAdjustment
{
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public string ReferenceNo { get; set; } = string.Empty;

    public string CustomerOrSupplier { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string AdjustmentType { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal TotalAmountRecovered { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string AddedBy { get; set; } = string.Empty;

    public List<StockAdjustmentItem> Items { get; set; } = [];

    public void RecalculateTotals()
    {
        foreach (var item in Items)
        {
            item.SubTotal = item.Qty * item.UnitPrice;
        }

        TotalAmount = Items.Sum(x => x.SubTotal);
    }
}

public sealed class StockAdjustmentItem
{
    public int Id { get; set; }

    public int StockAdjustmentId { get; set; }

    public int ProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Qty { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal SubTotal { get; set; }
}
