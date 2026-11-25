using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Parsing;
using TelegramNavigator.Core.Services;
using TelegramNavigator.Desktop.Services;
using TelegramNavigator.Desktop.ViewModels;
using TelegramNavigator.Desktop.Views;

namespace TelegramNavigator.Desktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Composition root: создаём все зависимости вручную
            var mainWindow = new MainWindow();
            var parser = new TelegramJsonParser();
            var analyzer = new ChatAnalyzer();
            var session = new ChatSession();
            var dialogService = new DialogService(mainWindow);
            var viewModel = new MainWindowViewModel(parser, analyzer, session, dialogService);

            mainWindow.DataContext = viewModel;
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}