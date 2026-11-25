using Avalonia.Controls;
using Avalonia.Input;
using TelegramNavigator.Desktop.ViewModels;

namespace TelegramNavigator.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Подписываемся на событие загрузки DataContext для FocusSearch
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.FocusSearchRequested += () =>
            {
                // Ищем TextBox поиска по имени
                var searchBox = this.FindControl<TextBox>("SearchBox");
                searchBox?.Focus();
            };
        }
    }
}