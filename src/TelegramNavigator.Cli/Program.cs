using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Cli;
using TelegramNavigator.Core.Export;
using TelegramNavigator.Core.Parsing;
using TelegramNavigator.Core.Models;

// ===== Парсинг аргументов =====
var parseResult = CommandLineParser.Parse(args);

if (parseResult.Errors.Count > 0)
{
    Console.Error.WriteLine("Ошибки аргументов:");
    foreach (var err in parseResult.Errors)
        Console.Error.WriteLine($"  {err}");
    Console.Error.WriteLine();
    Console.Error.WriteLine(HelpText.Cli);
    return 1;
}

var options = parseResult.Options;

// --help
if (options.ShowHelp)
{
    Console.WriteLine(HelpText.Cli);
    return 0;
}

// --path обязателен для работы
if (options.Path is null)
{
    Console.Error.WriteLine("Ошибка: укажите --path <путь к result.json>");
    Console.Error.WriteLine(HelpText.Cli);
    return 1;
}

// ===== Парсинг файла =====
IChatParser parser = new TelegramJsonParser();
Chat chat;
try
{
    chat = await parser.ParseFileAsync(options.Path);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Ошибка парсинга файла: {ex.Message}");
    return 2;
}

var messages = chat.Messages.AsEnumerable();

// Применяем фильтр, если указан
var filter = options.ToFilter();
if (filter is not null)
    messages = messages.Where(m => filter.Matches(m));

var filteredList = messages.ToList();

// ===== Вывод сводки =====
IChatAnalyzer analyzer = new ChatAnalyzer();
var summary = analyzer.GetSummary(filteredList);
Console.WriteLine($"Чат: {chat.Name}");
Console.WriteLine($"Тип: {chat.Type}");
Console.WriteLine($"Всего сообщений (с учётом фильтра): {summary.TotalMessages}");
Console.WriteLine($"Участников: {summary.Participants}");
Console.WriteLine($"Медиа: {summary.MediaCount}");
Console.WriteLine($"Ссылок: {summary.LinkCount}");
if (summary.First is not null)
    Console.WriteLine($"Период: {summary.First:dd.MM.yyyy} — {summary.Last:dd.MM.yyyy}");
Console.WriteLine();

// ===== Топ авторов =====
var topN = options.Top ?? 5;
var topAuthors = analyzer.GetTopAuthors(filteredList, topN);
Console.WriteLine($"Топ-{topN} авторов:");
foreach (var stat in topAuthors)
{
    Console.WriteLine($"  {stat.Author,-20} {stat.MessageCount,6} сообщ. ({stat.Percent}%)");
}

Console.WriteLine();

// ===== Экспорт в PDF =====
if (options.ExportPdf is not null)
{
    Console.WriteLine("Экспорт в PDF...");
    IChatExporter exporter = new PdfChatExporter();
    try
    {
        await exporter.ExportAsync(chat.Name, filteredList, options.ExportPdf);
        Console.WriteLine($"PDF сохранён: {options.ExportPdf}");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Ошибка экспорта PDF: {ex.Message}");
        return 2;
    }
}

return 0;