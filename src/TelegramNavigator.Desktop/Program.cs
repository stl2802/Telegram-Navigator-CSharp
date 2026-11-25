using Avalonia;
using TelegramNavigator.Core.Cli;
using TelegramNavigator.Core.Export;
using TelegramNavigator.Core.Parsing;
using TelegramNavigator.Core.Analytics;

namespace TelegramNavigator.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Разбираем аргументы командной строки
        var parseResult = CommandLineParser.Parse(args);

        // --help — печатаем и выходим (даже без Avalonia)
        if (parseResult.Options.ShowHelp)
        {
            Console.WriteLine(Core.Cli.HelpText.Hotkeys);
            return;
        }

        // --path + --export-pdf — экспорт без открытия окна
        if (parseResult.Options.Path is not null && parseResult.Options.ExportPdf is not null)
        {
            // На Windows с WinExe консольный вывод не виден.
            // Для полноценного CLI используйте проект TelegramNavigator.Cli.
            RunCliExport(parseResult.Options).GetAwaiter().GetResult();
            return;
        }

        // Обычный запуск с открытием окна
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static async Task RunCliExport(Core.Cli.CommandLineOptions options)
    {
        try
        {
            var parser = new TelegramJsonParser();
            var chat = await parser.ParseFileAsync(options.Path!);

            var messages = chat.Messages.AsEnumerable();
            var filter = options.ToFilter();
            if (filter is not null)
                messages = messages.Where(m => filter.Matches(m));

            var filteredList = messages.ToList();

            var exporter = new PdfChatExporter();
            await exporter.ExportAsync(chat.Name, filteredList, options.ExportPdf);

            Console.WriteLine($"PDF сохранён: {options.ExportPdf}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
