namespace TelegramNavigator.Core.Models;

/// <summary>
/// Базовый класс для всех типов сообщений в чате.
/// </summary>
/// <param name="Id">Уникальный идентификатор сообщения.</param>
/// <param name="Date">Дата и время отправки.</param>
/// <param name="Author">Имя автора (может быть пустой строкой, если автор неизвестен).</param>
/// <param name="Text">Текстовое содержимое сообщения.</param>
/// <param name="Links">Список ссылок, извлечённых из текста.</param>
/// <param name="ReplyToId">Идентификатор сообщения, на которое дан ответ (null, если ответа нет).</param>
public abstract record ChatMessage(
    long Id,
    DateTime Date,
    string Author,
    string Text,
    IReadOnlyList<LinkInfo> Links,
    long? ReplyToId)
{
    /// <summary>
    /// Краткая строка для отображения в списке сообщений.
    /// Зависит от типа сообщения: для текстового — первые символы,
    /// для медиа — тип файла, для служебного — описание действия.
    /// </summary>
    public abstract string Preview { get; }

    /// <summary>
    /// Есть ли у сообщения медиа-вложение.
    /// Возвращает false для базового класса; переопределяется в <see cref="MediaMessage"/>.
    /// </summary>
    public virtual bool HasMedia => false;
}