using JewelryBillingSoft.UI.ViewModels;
using System.Windows;
using System.Windows.Input;
using WpfApplication = System.Windows.Application;

namespace JewelryBillingSoft.UI;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            MaximizeRestoreWindow();
        else
            DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e) =>
        WindowState = WindowState.Minimized;

    private void MaximizeButton_Click(object sender, RoutedEventArgs e) =>
        MaximizeRestoreWindow();

    private void CloseButton_Click(object sender, RoutedEventArgs e) =>
        WpfApplication.Current.Shutdown();

    private void MaximizeRestoreWindow()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
}
