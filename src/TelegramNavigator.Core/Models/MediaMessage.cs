namespace TelegramNavigator.Core.Models;

/// <summary>
/// Сообщение с медиа-вложением: фото, видео, аудио, стикер, документ и т.д.
/// </summary>
/// <param name="Id">Уникальный идентификатор сообщения.</param>
/// <param name="Date">Дата и время отправки.</param>
/// <param name="Author">Имя автора.</param>
/// <param name="Text">Текст сообщения (подпись к медиа).</param>
/// <param name="Links">Список ссылок в тексте.</param>
/// <param name="ReplyToId">Идентификатор сообщения-ответа (null, если нет).</param>
/// <param name="Kind">Тип медиа-вложения.</param>
/// <param name="FilePath">Путь к файлу медиа (относительно папки экспорта).</param>
/// <param name="IsFileAvailable">Доступен ли файл (true, если файл был выгружен при экспорте).</param>
public sealed record MediaMessage(
    long Id,
    DateTime Date,
    string Author,
    string Text,
    IReadOnlyList<LinkInfo> Links,
    long? ReplyToId,
    MediaKind Kind,
    string? FilePath,
    bool IsFileAvailable)
    : ChatMessage(Id, Date, Author, Text, Links, ReplyToId)
{
    /// <summary>
    /// Строка вида "[Фото]" или "[Видео: video.mp4]", описывающая тип медиа.
    /// </summary>
    public override string Preview
    {
        get
        {
            var kindLabel = Kind switch
            {
                MediaKind.Photo => "Фото",
                MediaKind.Video => "Видео",
                MediaKind.VideoMessage => "Кружок",
                MediaKind.VoiceMessage => "Голосовое",
                MediaKind.Audio => "Аудио",
                MediaKind.Sticker => "Стикер",
                MediaKind.Animation => "GIF",
                MediaKind.Document => "Документ",
                _ => "Медиа"
            };
            var fileName = FilePath is not null ? $": {Path.GetFileName(FilePath)}" : "";
            return $"[{kindLabel}{fileName}]";
        }
    }

    /// <summary>
    /// У медиа-сообщения всегда есть вложение.
    /// </summary>
    public override bool HasMedia => true;
}