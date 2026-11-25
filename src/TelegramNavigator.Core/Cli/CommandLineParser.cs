using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Cli;

/// <summary>
/// Результат разбора аргументов командной строки.
/// </summary>
/// <param name="Options">Разобранные опции (могут быть неполными при ошибках).</param>
/// <param name="Errors">Список ошибок разбора (пустой, если ошибок нет).</param>
public record ParseResult(CommandLineOptions Options, IReadOnlyList<string> Errors);

/// <summary>
/// Примитивный парсер командной строки без внешних зависимостей.
/// Не кидает исключений — все ошибки аккумулируются в <see cref="ParseResult.Errors"/>.
/// </summary>
public static class CommandLineParser
{
    /// <summary>
    /// Разбирает аргументы командной строки.
    /// </summary>
    public static ParseResult Parse(string[] args)
    {
        var errors = new List<string>();
        string? path = null;
        string? exportPdf = null;
        string? author = null;
        string? search = null;
        DateOnly? from = null;
        DateOnly? to = null;
        int? top = null;
        bool showHelp = false;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg is "--help" or "-h" or "/?")
            {
                showHelp = true;
                continue;
            }

            if (arg.StartsWith("--"))
            {
                var value = (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    ? args[++i]
                    : null;

                switch (arg)
                {
                    case "--path":
                        if (value is null)
                            errors.Add("--path: ожидается значение (путь к файлу).");
                        else
                            path = value;
                        break;

                    case "--export-pdf":
                        if (value is null)
                            errors.Add("--export-pdf: ожидается значение (путь для экспорта).");
                        else
                            exportPdf = value;
                        break;

                    case "--author":
                        author = value;
                        break;

                    case "--search":
                        search = value;
                        break;

                    case "--from":
                        if (value is null)
                            errors.Add("--from: ожидается значение (yyyy-MM-dd).");
                        else if (!DateOnly.TryParse(value, out var fd))
                            errors.Add($"--from: неверный формат даты '{value}'. Ожидается yyyy-MM-dd.");
                        else
                            from = fd;
                        break;

                    case "--to":
                        if (value is null)
                            errors.Add("--to: ожидается значение (yyyy-MM-dd).");
                        else if (!DateOnly.TryParse(value, out var td))
                            errors.Add($"--to: неверный формат даты '{value}'. Ожидается yyyy-MM-dd.");
                        else
                            to = td;
                        break;

                    case "--top":
                        if (value is null)
                            errors.Add("--top: ожидается значение (число).");
                        else if (!int.TryParse(value, out var n) || n <= 0)
                            errors.Add($"--top: неверное значение '{value}'. Ожидается положительное целое число.");
                        else
                            top = n;
                        break;

                    default:
                        errors.Add($"Неизвестный флаг: {arg}");
                        break;
                }
            }
            else
            {
                errors.Add($"Неизвестный аргумент: {arg}");
            }
        }

        var options = new CommandLineOptions(
            Path: path,
            ExportPdf: exportPdf,
            Author: author,
            Search: search,
            From: from,
            To: to,
            Top: top,
            ShowHelp: showHelp);

        return new ParseResult(options, errors.AsReadOnly());
    }

    /// <summary>
    /// Преобразует опции командной строки в фильтр сообщений.
    /// </summary>
    public static MessageFilter? ToFilter(this CommandLineOptions options)
    {
        // Если нет ни одного фильтрующего параметра — возвращаем null (фильтр не нужен)
        if (options.Author is null && options.Search is null
            && options.From is null && options.To is null)
            return null;

        return new MessageFilter(
            SearchText: options.Search,
            Author: options.Author,
            From: options.From,
            To: options.To,
            OnlyWithMedia: false,
            OnlyWithLinks: false);
    }
}