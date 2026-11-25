namespace TelegramNavigator.Core.Cli;

/// <summary>
/// Разобранные параметры командной строки.
/// </summary>
/// <param name="Path">Путь к файлу result.json.</param>
/// <param name="ExportPdf">Путь для экспорта PDF (null — не экспортировать).</param>
/// <param name="Author">Фильтр по автору.</param>
/// <param name="Search">Фильтр по тексту.</param>
/// <param name="From">Начальная дата (включительно).</param>
/// <param name="To">Конечная дата (включительно).</param>
/// <param name="Top">Количество авторов в топе (по умолчанию 5).</param>
/// <param name="ShowHelp">Показать справку.</param>
public record CommandLineOptions(
    string? Path,
    string? ExportPdf,
    string? Author,
    string? Search,
    DateOnly? From,
    DateOnly? To,
    int? Top,
    bool ShowHelp);