using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Services;

/// <summary>
/// Аргументы события о загрузке нового чата.
/// </summary>
public class ChatLoadedEventArgs : EventArgs
{
    /// <summary>
    /// Загруженный чат.
    /// </summary>
    public Chat Chat { get; }

    /// <summary>
    /// Создаёт аргументы с переданным чатом.
    /// </summary>
    public ChatLoadedEventArgs(Chat chat)
    {
        Chat = chat;
    }
}