namespace OrderWiseErp.App.Models;

public sealed class Project
{
    public int Id { get; set; }

    public string ProjectNo { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
