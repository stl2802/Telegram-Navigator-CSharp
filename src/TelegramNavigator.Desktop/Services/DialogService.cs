using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using TelegramNavigator.Desktop.Views;

namespace TelegramNavigator.Desktop.Services;

/// <summary>
/// Реализация диалогов через Avalonia Window.
/// </summary>
public class DialogService : IDialogService
{
    private readonly Window _window;

    public DialogService(Window window)
    {
        _window = window;
    }

    public async Task<string?> PickChatFileAsync()
    {
        var files = await _window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите файл result.json",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("JSON-экспорт Telegram")
                {
                    Patterns = new[] { "*.json" }
                }
            },
        });

        return files.Count >= 1 ? files[0].Path.LocalPath : null;
    }

    public async Task<string?> PickPdfSavePathAsync(string suggestedName)
    {
        var result = await _window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Сохранить PDF",
            SuggestedFileName = suggestedName,
            FileTypeChoices = new[]
            {
                new FilePickerFileType("PDF-документ")
                {
                    Patterns = new[] { "*.pdf" }
                }
            },
        });

        return result?.Path.LocalPath;
    }

    public async Task ShowHelpAsync()
    {
        var helpWindow = new HelpWindow();
        await helpWindow.ShowDialog(_window);
    }

    public async Task ShowErrorAsync(string title, string message)
    {
        var msgLabel = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 400,
        };

        var okButton = new Button
        {
            Content = "OK",
            HorizontalAlignment = HorizontalAlignment.Center,
            Width = 100,
        };

var content = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 15,
        };
        content.Children.Add(msgLabel);
        content.Children.Add(okButton);
        var dialog = new Window
        {
            Title = title,
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = content,
        };

        okButton.Click += (_, _) => dialog.Close();

        await dialog.ShowDialog(_window);
    }
}