namespace TelegramNavigator.Core.Settings;

/// <summary>
/// Настройки приложения, сохраняемые между сеансами.
/// </summary>
/// <param name="Theme">Название темы оформления (например, "Light", "Dark").</param>
/// <param name="LastPath">Последний открытый файл.</param>
/// <param name="SearchText">Последний введённый текст поиска.</param>
/// <param name="Author">Последний отфильтрованный автор.</param>
/// <param name="OnlyWithMedia">Флаг фильтра «только с медиа».</param>
/// <param name="OnlyWithLinks">Флаг фильтра «только со ссылками».</param>
/// <param name="RecentFiles">Список недавно открытых файлов.</param>
public record AppSettings(
    string Theme,
    string? LastPath,
    string? SearchText,
    string? Author,
    bool OnlyWithMedia,
    bool OnlyWithLinks,
    IReadOnlyList<string> RecentFiles)
{
    /// <summary>
    /// Настройки по умолчанию.
    /// </summary>
    public static AppSettings Default { get; } = new(
        Theme: "Light",
        LastPath: null,
        SearchText: null,
        Author: null,
        OnlyWithMedia: false,
        OnlyWithLinks: false,
        RecentFiles: Array.Empty<string>());
}