using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Services;

/// <summary>
/// Текущая сессия работы с чатом: хранит загруженный чат, фильтр, 
/// коллекции отфильтрованных сообщений, медиа и ссылок.
/// </summary>
public class ChatSession
{
    private Chat? _currentChat;
    private List<ChatMessage> _filteredMessages = new();
    private List<ChatMessage> _media = new();
    private List<LinkEntry> _links = new();

    /// <summary>
    /// Текущий загруженный чат (null, если чат не загружен).
    /// </summary>
    public Chat? CurrentChat => _currentChat;

    /// <summary>
    /// Текущий активный фильтр.
    /// </summary>
    public MessageFilter Filter { get; private set; } = MessageFilter.Empty;

    /// <summary>
    /// Сообщения, прошедшие через текущий фильтр.
    /// </summary>
    public IReadOnlyList<ChatMessage> FilteredMessages => _filteredMessages;

    /// <summary>
    /// Все сообщения с медиа-вложениями (без фильтра, из загруженного чата).
    /// </summary>
    public IReadOnlyList<ChatMessage> Media => _media;

    /// <summary>
    /// Все пары «сообщение — ссылка» из загруженного чата.
    /// </summary>
    public IReadOnlyList<LinkEntry> Links => _links;

    /// <summary>
    /// Событие: загружен новый чат.
    /// </summary>
    public event EventHandler<ChatLoadedEventArgs>? ChatLoaded;

    /// <summary>
    /// Событие: изменился фильтр (и, соответственно, список отфильтрованных сообщений).
    /// </summary>
    public event EventHandler? FilterChanged;

    /// <summary>
    /// Загружает новый чат и пересчитывает все коллекции.
    /// </summary>
    public void Load(Chat chat)
    {
        _currentChat = chat;

        // Кэшируем медиа и ссылки в плоские списки
        _media = chat.Messages
            .Where(m => m.HasMedia)
            .ToList();

        _links = chat.Messages
            .SelectMany(m => m.Links.Select(link => new LinkEntry(m, link)))
            .ToList();

        // Применяем текущий фильтр (по умолчанию Empty — пропускает всё)
        ApplyFilterCore();

        ChatLoaded?.Invoke(this, new ChatLoadedEventArgs(chat));
    }

    /// <summary>
    /// Применяет новый фильтр к уже загруженному чату.
    /// </summary>
    public void ApplyFilter(MessageFilter filter)
    {
        Filter = filter;
        ApplyFilterCore();
        FilterChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Очищает сессию: сбрасывает чат, фильтр и все коллекции.
    /// </summary>
    public void Clear()
    {
        _currentChat = null;
        Filter = MessageFilter.Empty;
        _filteredMessages.Clear();
        _media.Clear();
        _links.Clear();
        FilterChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyFilterCore()
    {
        if (_currentChat is null)
        {
            _filteredMessages.Clear();
            return;
        }

        _filteredMessages = _currentChat.Messages
            .Where(m => Filter.Matches(m))
            .ToList();
    }
}

/// <summary>
/// Пара: сообщение и одна ссылка из этого сообщения.
/// </summary>
/// <param name="Message">Сообщение, содержащее ссылку.</param>
/// <param name="Link">Информация о ссылке.</param>
public record LinkEntry(ChatMessage Message, LinkInfo Link);