namespace TelegramNavigator.Core.Models;

/// <summary>
/// Тип медиа, вложенного в сообщение.
/// </summary>
public enum MediaKind
{
    /// <summary>Нет медиа (используется в TextMessage и ServiceMessage).</summary>
    None,
    /// <summary>Фотография.</summary>
    Photo,
    /// <summary>Видео.</summary>
    Video,
    /// <summary>Кружок (видеосообщение).</summary>
    VideoMessage,
    /// <summary>Голосовое сообщение.</summary>
    VoiceMessage,
    /// <summary>Аудиофайл.</summary>
    Audio,
    /// <summary>Стикер.</summary>
    Sticker,
    /// <summary>GIF / анимация.</summary>
    Animation,
    /// <summary>Произвольный документ.</summary>
    Document,
}