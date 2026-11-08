namespace TelegramNavigator.Core.Models;

/// <summary>
/// Чат целиком: название, тип, уникальный идентификатор и список сообщений.
/// </summary>
/// <param name="Name">Название чата.</param>
/// <param name="Type">Тип чата (например, "personal_chat", "private_group", "public_supergroup").</param>
/// <param name="Id">Уникальный идентификатор чата.</param>
/// <param name="Messages">Все сообщения чата.</param>
public record Chat(
    string Name,
    string Type,
    long Id,
    IReadOnlyList<ChatMessage> Messages)
{
    /// <summary>
    /// Количество уникальных авторов.
    /// </summary>
    public int Participants => Messages
        .Select(m => m.Author)
        .Where(a => !string.IsNullOrEmpty(a))
        .Distinct()
        .Count();

    /// <summary>
    /// Дата самого первого сообщения или null, если сообщений нет.
    /// </summary>
    public DateTime? FirstDate => Messages.Count > 0
        ? Messages.Min(m => m.Date)
        : null;

    /// <summary>
    /// Дата самого последнего сообщения или null, если сообщений нет.
    /// </summary>
    public DateTime? LastDate => Messages.Count > 0
        ? Messages.Max(m => m.Date)
        : null;
}