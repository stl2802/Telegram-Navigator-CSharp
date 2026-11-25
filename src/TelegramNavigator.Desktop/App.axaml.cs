using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Export;
using TelegramNavigator.Core.Parsing;
using TelegramNavigator.Core.Services;
using TelegramNavigator.Core.Settings;
using TelegramNavigator.Desktop.Services;
using TelegramNavigator.Desktop.ViewModels;
using TelegramNavigator.Desktop.Views;

namespace TelegramNavigator.Desktop;

public partial class App : Application
{
    private MainWindowViewModel? _viewModel;

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
            var exporter = new PdfChatExporter();
            var settings = new JsonSettingsService();

            _viewModel = new MainWindowViewModel(
                parser, analyzer, session, dialogService, exporter, settings);

            // Загружаем настройки
            var appSettings = settings.Load();
            _viewModel.ApplySettings(appSettings);

            // Если в настройках есть LastPath и файл существует — открываем
            if (appSettings.LastPath is not null && File.Exists(appSettings.LastPath))
            {
                _ = _viewModel.LoadFileAsync(appSettings.LastPath);
            }

            mainWindow.DataContext = _viewModel;
            desktop.MainWindow = mainWindow;

            // Сохраняем настройки при закрытии
            desktop.ShutdownRequested += OnShutdownRequested;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        if (_viewModel is null) return;

        var settings = new JsonSettingsService();
        var appSettings = _viewModel.CreateSettings();
        settings.Save(appSettings);
    }
}