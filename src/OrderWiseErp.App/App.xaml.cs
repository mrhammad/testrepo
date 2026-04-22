using System.IO;
using System.Windows;
using OrderWiseErp.App.Services;
using OrderWiseErp.App.ViewModels;

namespace OrderWiseErp.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var dataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OrderWiseErp");
        var dataFilePath = Path.Combine(dataFolder, "phase1-data.json");
        var settingsFilePath = Path.Combine(dataFolder, "settings.json");
        IAppDataService dataService = new JsonFileAppDataService(dataFilePath);
        var localizationService = LocalizationService.Instance;
        localizationService.Initialize(settingsFilePath);
        var mainViewModel = new MainViewModel(dataService, localizationService);
        var mainWindow = new MainWindow(mainViewModel);
        mainWindow.Show();
    }
}
