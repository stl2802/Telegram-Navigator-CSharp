namespace TelegramNavigator.Core.Models;

/// <summary>
/// Служебное сообщение: уведомление о действиях участников
/// (например, «Иван закрепил сообщение», «Пётр добавил Марию»).
/// </summary>
/// <param name="Id">Уникальный идентификатор сообщения.</param>
/// <param name="Date">Дата и время.</param>
/// <param name="Author">Имя автора действия.</param>
/// <param name="Text">Текстовое описание действия.</param>
/// <param name="Links">Список ссылок (обычно пустой).</param>
/// <param name="ReplyToId">Всегда null для служебных сообщений.</param>
/// <param name="Action">Тип действия (например, "pin", "add_user", "remove_user").</param>
public sealed record ServiceMessage(
    long Id,
    DateTime Date,
    string Author,
    string Text,
    IReadOnlyList<LinkInfo> Links,
    long? ReplyToId,
    string Action)
    : ChatMessage(Id, Date, Author, Text, Links, ReplyToId)
{
    /// <summary>
    /// Строка вида «[Служебное: pin]».
    /// </summary>
    public override string Preview => $"[Служебное: {Action}]";
}