using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Export;

/// <summary>
/// Экспорт сообщений чата в файл (PDF).
/// </summary>
public interface IChatExporter
{
    /// <summary>
    /// Асинхронно экспортирует список сообщений в файл.
    /// </summary>
    /// <param name="chatName">Название чата для заголовка экспорта.</param>
    /// <param name="messages">Список сообщений для экспорта.</param>
    /// <param name="outputPath">Путь к выходному файлу.</param>
    /// <param name="ct">Токен отмены.</param>
    Task ExportAsync(string chatName, IReadOnlyList<ChatMessage> messages, string outputPath, CancellationToken ct = default);
}