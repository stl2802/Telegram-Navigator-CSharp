namespace TelegramNavigator.Core.Models;

/// <summary>
/// Обычное текстовое сообщение (без медиа-вложения).
/// </summary>
/// <param name="Id">Уникальный идентификатор сообщения.</param>
/// <param name="Date">Дата и время отправки.</param>
/// <param name="Author">Имя автора.</param>
/// <param name="Text">Текст сообщения.</param>
/// <param name="Links">Список ссылок в тексте.</param>
/// <param name="ReplyToId">Идентификатор сообщения-ответа (null, если нет).</param>
public sealed record TextMessage(
    long Id,
    DateTime Date,
    string Author,
    string Text,
    IReadOnlyList<LinkInfo> Links,
    long? ReplyToId)
    : ChatMessage(Id, Date, Author, Text, Links, ReplyToId)
{
    /// <summary>
    /// Первые 100 символов текста, с многоточием, если текст длиннее.
    /// </summary>
    public override string Preview => Text.Length <= 100
        ? Text
        : string.Concat(Text.AsSpan(0, 97), "...");
}