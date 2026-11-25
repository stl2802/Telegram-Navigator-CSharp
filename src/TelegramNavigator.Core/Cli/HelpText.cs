using System.Text;

namespace TelegramNavigator.Core.Cli;

/// <summary>
/// Тексты справки для CLI и горячих клавиш Desktop.
/// </summary>
public static class HelpText
{
    /// <summary>
    /// Справка по использованию CLI.
    /// </summary>
    public static string Cli { get; } = GetCliHelp();

    /// <summary>
    /// Справка по горячим клавишам Desktop-версии.
    /// </summary>
    public static string Hotkeys { get; } = GetHotkeysHelp();

    private static string GetCliHelp()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Telegram Navigator — CLI");
        sb.AppendLine("Использование:");
        sb.AppendLine("  TelegramNavigator.Cli --path <result.json> [опции]");
        sb.AppendLine();
        sb.AppendLine("Опции:");
        sb.AppendLine("  --path <путь>          Путь к файлу result.json (обязательный)");
        sb.AppendLine("  --export-pdf <путь>     Экспорт отфильтрованных сообщений в PDF");
        sb.AppendLine("  --author <имя>          Фильтр по автору");
        sb.AppendLine("  --search <текст>        Поиск по тексту сообщений");
        sb.AppendLine("  --from <yyyy-MM-dd>     Начальная дата (включительно)");
        sb.AppendLine("  --to <yyyy-MM-dd>       Конечная дата (включительно)");
        sb.AppendLine("  --top <N>               Количество авторов в топе (по умолчанию 5)");
        sb.AppendLine("  --help, -h, /?          Показать эту справку");
        sb.AppendLine();
        sb.AppendLine("Примеры:");
        sb.AppendLine("  TelegramNavigator.Cli --path result.json");
        sb.AppendLine("  TelegramNavigator.Cli --path result.json --top 10");
        sb.AppendLine("  TelegramNavigator.Cli --path result.json --author Иван --from 2024-01-01 --to 2024-06-30");
        sb.AppendLine("  TelegramNavigator.Cli --path result.json --export-pdf out.pdf");
        sb.AppendLine();
        sb.AppendLine("Коды возврата: 0 — успех, 1 — ошибка аргументов, 2 — ошибка парсинга/экспорта");
        return sb.ToString();
    }

    private static string GetHotkeysHelp()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Telegram Navigator — Desktop");
        sb.AppendLine();
        sb.AppendLine("Горячие клавиши:");
        sb.AppendLine("  F1         — справка");
        sb.AppendLine("  Ctrl+O     — открыть файл result.json");
        sb.AppendLine("  Ctrl+F     — поиск");
        sb.AppendLine("  Ctrl+E     — экспорт в PDF");
        sb.AppendLine("  Ctrl+T     — переключить тему (светлая / тёмная)");
        sb.AppendLine("  Ctrl+A     — выбрать все сообщения для экспорта");
        sb.AppendLine("  Escape     — сбросить фильтры");
        sb.AppendLine();
        sb.AppendLine("Аргументы командной строки (для Windows — использовать Cli-версию):");
        sb.AppendLine("  --path <путь>           Открыть указанный файл при старте");
        sb.AppendLine("  --export-pdf <путь>     Экспорт в PDF и выход (только с --path)");
        sb.AppendLine("  --help / -h /?          Показать эту справку");
        return sb.ToString();
    }
}