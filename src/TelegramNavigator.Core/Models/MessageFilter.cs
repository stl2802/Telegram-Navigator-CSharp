namespace TelegramNavigator.Core.Models;

/// <summary>
/// Фильтр для поиска и отбора сообщений.
/// Все условия объединяются по логике "И".
/// </summary>
/// <param name="SearchText">Текст для поиска (без учёта регистра).</param>
/// <param name="Author">Фильтр по автору (без учёта регистра).</param>
/// <param name="From">Начальная дата (включительно).</param>
/// <param name="To">Конечная дата (включительно).</param>
/// <param name="OnlyWithMedia">Показывать только сообщения с медиа.</param>
/// <param name="OnlyWithLinks">Показывать только сообщения со ссылками.</param>
public record MessageFilter(
    string? SearchText,
    string? Author,
    DateOnly? From,
    DateOnly? To,
    bool OnlyWithMedia,
    bool OnlyWithLinks)
{
    /// <summary>
    /// Пустой фильтр (пропускает все сообщения).
    /// </summary>
    public static MessageFilter Empty => new(null, null, null, null, false, false);

    /// <summary>
    /// Проверяет, проходит ли сообщение через этот фильтр.
    /// </summary>
    public bool Matches(ChatMessage message)
    {
        // Поиск по тексту (без учёта регистра)
        if (SearchText is not null && SearchText.Length > 0)
        {
            if (message.Text.Length == 0) return false;
            if (message.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == false)
                return false;
        }

        // Фильтр по автору (без учёта регистра)
        if (Author is not null && Author.Length > 0)
        {
            if (message.Author.Length == 0) return false;
            if (message.Author.Contains(Author, StringComparison.OrdinalIgnoreCase) == false)
                return false;
        }

        // Фильтр по дате (границы включительно)
        if (From is not null)
        {
            var messageDate = DateOnly.FromDateTime(message.Date);
            if (messageDate < From.Value) return false;
        }

        if (To is not null)
        {
            var messageDate = DateOnly.FromDateTime(message.Date);
            if (messageDate > To.Value) return false;
        }

        // Только с медиа
        if (OnlyWithMedia && !message.HasMedia)
            return false;

        // Только с ссылками
        if (OnlyWithLinks && message.Links.Count == 0)
            return false;

        return true;
    }
}