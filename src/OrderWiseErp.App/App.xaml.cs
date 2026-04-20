using System.Windows;
using OrderWiseErp.App.Services;
using OrderWiseErp.App.ViewModels;

namespace OrderWiseErp.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IAppDataService dataService = new InMemoryAppDataService();
        var mainViewModel = new MainViewModel(dataService);
        var mainWindow = new MainWindow(mainViewModel);
        mainWindow.Show();
    }
}
