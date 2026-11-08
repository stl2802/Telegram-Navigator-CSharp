using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Parsing;

/// <summary>
/// Парсинг чата из JSON-экспорта Telegram Desktop.
/// </summary>
public interface IChatParser
{
    /// <summary>
    /// Асинхронно разбирает JSON-поток в доменную модель <see cref="Chat"/>.
    /// </summary>
    /// <param name="stream">Поток с содержимым result.json.</param>
    /// <param name="baseDirectory">Базовая директория для разрешения относительных путей медиа.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Разобранный чат.</returns>
    Task<Chat> ParseAsync(Stream stream, string? baseDirectory, CancellationToken ct = default);

    /// <summary>
    /// Открывает файл и вызывает <see cref="ParseAsync"/>.
    /// </summary>
    /// <param name="path">Полный путь к файлу result.json.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Разобранный чат.</returns>
    Task<Chat> ParseFileAsync(string path, CancellationToken ct = default);
}