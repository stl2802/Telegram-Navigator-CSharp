namespace TelegramNavigator.Core.Models;

/// <summary>
/// Исключение, выбрасываемое при ошибке парсинга чата из JSON.
/// </summary>
public class ChatParseException : Exception
{
    /// <summary>
    /// Создаёт исключение с сообщением об ошибке.
    /// </summary>
    public ChatParseException(string message) : base(message) { }

    /// <summary>
    /// Создаёт исключение с сообщением и внутренним исключением.
    /// </summary>
    public ChatParseException(string message, Exception innerException)
        : base(message, innerException) { }
}