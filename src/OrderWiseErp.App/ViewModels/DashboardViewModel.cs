using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class DashboardViewModel : ObservableObject
{
    private readonly IAppDataService _dataService;
    private readonly ObservableCollection<DashboardTile> _tiles = [];

    public DashboardViewModel(IAppDataService dataService)
    {
        _dataService = dataService;
        Tiles = new ReadOnlyObservableCollection<DashboardTile>(_tiles);
        Refresh();
    }

    public ReadOnlyObservableCollection<DashboardTile> Tiles { get; }

    public void Refresh()
    {
        _tiles.Clear();
        _tiles.Add(new DashboardTile("Projects", _dataService.Projects.Count.ToString()));
        _tiles.Add(new DashboardTile("Contacts", _dataService.Contacts.Count.ToString()));
        _tiles.Add(new DashboardTile("Products", _dataService.Products.Count.ToString()));
        _tiles.Add(new DashboardTile("Purchases", _dataService.Purchases.Count.ToString()));
        _tiles.Add(new DashboardTile("Sales", _dataService.Sales.Count.ToString()));
        _tiles.Add(new DashboardTile("Modules Implemented", "7"));
        _tiles.Add(new DashboardTile("Next Modules", "Payments / Expenses / Stock / Reports"));
    }

    public sealed class DashboardTile
    {
        public DashboardTile(string label, string value)
        {
            Label = label;
            Value = value;
        }

        public string Label { get; }

        public string Value { get; }
    }
}
