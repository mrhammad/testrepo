using System.Windows;
using OrderWiseErp.App.ViewModels;

namespace OrderWiseErp.App;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
