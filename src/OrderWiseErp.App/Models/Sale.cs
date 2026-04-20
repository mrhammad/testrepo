namespace OrderWiseErp.App.Models;

public sealed class Sale
{
    public int Id { get; set; }

    public string InvoiceNo { get; set; } = string.Empty;

    public int ProjectId { get; set; }

    public string ProjectNo { get; set; } = string.Empty;

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public DateTime? DueDate { get; set; }

    public int TermDays { get; set; }

    public string PoNumber { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal Balance { get; set; }

    public List<SaleItem> Items { get; set; } = [];

    public void RecalculateTotals()
    {
        foreach (var item in Items)
        {
            item.Amount = item.Qty * item.Rate;
            item.DiscountAmount = item.Amount * (item.DiscountPercent / 100m);
            var taxable = item.Amount - item.DiscountAmount;
            item.VatAmount = taxable * (item.VatRate / 100m);
            item.NetAmount = taxable + item.VatAmount;
        }

        TotalAmount = Items.Sum(x => x.NetAmount);
        Balance = TotalAmount;
    }
}
