using Avalonia.Controls;
using Ekz.ViewModels;

namespace Ekz.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel();
        InitializeComponent();
        
    }
}