namespace OrderWiseErp.App.Models;

public sealed class Purchase
{
    public int Id { get; set; }

    public string PurchaseNo { get; set; } = string.Empty;

    public string ProjectNo { get; set; } = string.Empty;

    public int ProjectId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public int SupplierId { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public DateTime? DueDate { get; set; }

    public int TermDays { get; set; }

    public string PoNumber { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal Balance { get; set; }

    public int CreatedBy { get; set; }

    public List<PurchaseItem> Items { get; set; } = [];

    public void RecalculateTotals()
    {
        foreach (var item in Items)
        {
            item.Amount = item.Qty * item.Rate;
            item.VatAmount = item.Amount * (item.VatRate / 100m);
            item.IncomeTaxAmount = item.Amount * (item.IncomeTaxRate / 100m);
            item.NetAmount = item.Amount + item.CustomDutyAmount + item.VatAmount + item.IncomeTaxAmount;
        }

        TotalAmount = Items.Sum(x => x.NetAmount);
        Balance = TotalAmount;
    }
}
