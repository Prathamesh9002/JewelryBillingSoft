using JewelryBillingSoft.UI.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace JewelryBillingSoft.UI.Views;

public partial class BillingView : UserControl
{
    public BillingView() => InitializeComponent();

    private void InvoicesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is BillingViewModel vm && vm.ViewInvoiceCommand.CanExecute(null))
        {
            var grid = (DataGrid)sender;
            if (grid.SelectedItem is Application.DTOs.InvoiceSummaryDto selected)
                vm.ViewInvoiceCommand.Execute(selected);
        }
    }
}

