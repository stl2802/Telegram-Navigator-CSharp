using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Tests.Models;

/// <summary>
/// Тесты для record <see cref="Chat"/>.
/// </summary>
public class ChatTests
{
    private static Chat CreateTestChat()
    {
        var date1 = new DateTime(2024, 1, 1, 10, 0, 0);
        var date2 = new DateTime(2024, 6, 15, 14, 30, 0);
        var date3 = new DateTime(2024, 12, 31, 23, 59, 0);

        var messages = new List<ChatMessage>
        {
            new TextMessage(1, date1, "Alice", "First", Array.Empty<LinkInfo>(), null),
            new MediaMessage(2, date2, "Bob", "Photo!", Array.Empty<LinkInfo>(), null,
                MediaKind.Photo, "photo.jpg", true),
            new TextMessage(3, date3, "Alice", "Last", Array.Empty<LinkInfo>(), null),
        };

        return new Chat("TestGroup", "private_group", 12345L, messages);
    }

    [Fact]
    public void Participants_ShouldCountUniqueAuthors()
    {
        var chat = CreateTestChat();
        Assert.Equal(2, chat.Participants);
    }

    [Fact]
    public void Participants_ShouldExcludeEmptyAuthors()
    {
        var messages = new List<ChatMessage>
        {
            new TextMessage(1, DateTime.Now, "", "Empty author", Array.Empty<LinkInfo>(), null),
            new TextMessage(2, DateTime.Now, "Alice", "Normal", Array.Empty<LinkInfo>(), null),
        };
        var chat = new Chat("Chat", "type", 1L, messages);
        Assert.Equal(1, chat.Participants);
    }

    [Fact]
    public void FirstDate_ShouldReturnMinDate()
    {
        var chat = CreateTestChat();
        Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0), chat.FirstDate);
    }

    [Fact]
    public void LastDate_ShouldReturnMaxDate()
    {
        var chat = CreateTestChat();
        Assert.Equal(new DateTime(2024, 12, 31, 23, 59, 0), chat.LastDate);
    }

    [Fact]
    public void Dates_ShouldBeNullForEmptyChat()
    {
        var chat = new Chat("Empty", "type", 1L, Array.Empty<ChatMessage>());
        Assert.Null(chat.FirstDate);
        Assert.Null(chat.LastDate);
    }
}