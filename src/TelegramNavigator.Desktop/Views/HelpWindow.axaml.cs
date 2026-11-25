using Avalonia.Controls;
using TelegramNavigator.Desktop.ViewModels;

namespace TelegramNavigator.Desktop.Views;

public partial class HelpWindow : Window
{
    public HelpWindow()
    {
        InitializeComponent();
        DataContext = new HelpViewModel();
    }
}