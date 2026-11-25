using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Tests.Helpers;

/// <summary>
/// Фабричные методы для создания тестовых сообщений.
/// </summary>
internal static class TestMessages
{
    private static readonly LinkInfo[] NoLinks = Array.Empty<LinkInfo>();
    private static readonly LinkInfo[] SingleLink = { new("https://example.com", "Example") };

    internal static ChatMessage Text(string text = "Hello", string author = "Alice",
        int year = 2024, int month = 6, int day = 15, int hour = 12, long id = 1)
    {
        return new TextMessage(id, new DateTime(year, month, day, hour, 0, 0),
            author, text, NoLinks, null);
    }

    internal static ChatMessage Media(string author = "Bob",
        MediaKind kind = MediaKind.Photo, string? filePath = "photo.jpg",
        int year = 2024, int month = 6, int day = 16, long id = 2)
    {
        return new MediaMessage(id, new DateTime(year, month, day, 14, 0, 0),
            author, "Photo", NoLinks, null, kind, filePath, filePath is not null);
    }

    internal static ChatMessage WithLinks(string text = "Check link", string author = "Charlie",
        int year = 2024, int month = 6, int day = 17, long id = 3)
    {
        return new TextMessage(id, new DateTime(year, month, day, 10, 0, 0),
            author, text, SingleLink, null);
    }

    internal static ChatMessage Service(string action = "pin", string author = "System",
        int year = 2024, int month = 6, int day = 18, long id = 100)
    {
        return new ServiceMessage(id, new DateTime(year, month, day, 10, 0, 0),
            author, $"Action: {action}", NoLinks, null, action);
    }

    internal static ChatMessage EmptyText() =>
        new TextMessage(10, new DateTime(2024, 6, 1), "Author", "", NoLinks, null);

    internal static ChatMessage EmptyAuthor() =>
        new TextMessage(11, new DateTime(2024, 6, 1), "", "Text", NoLinks, null);

    internal static IReadOnlyList<ChatMessage> ThreeMessages()
    {
        return new List<ChatMessage>
        {
            Text(),
            Media(),
            WithLinks(),
        };
    }
}