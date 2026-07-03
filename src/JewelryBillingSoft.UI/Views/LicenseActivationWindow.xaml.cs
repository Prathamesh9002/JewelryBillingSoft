using JewelryBillingSoft.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace JewelryBillingSoft.UI.Views;

public partial class LicenseActivationWindow : Window
{
    public LicenseActivationWindow(LicenseViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.ActivationSucceeded += OnActivationSucceeded;
    }

    private void OnActivationSucceeded(object? sender, EventArgs e)
    {
        var loginWindow = App.Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();
        Close();
    }
}

