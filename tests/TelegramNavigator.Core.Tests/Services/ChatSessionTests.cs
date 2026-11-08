using TelegramNavigator.Core.Models;
using TelegramNavigator.Core.Services;

namespace TelegramNavigator.Core.Tests.Services;

/// <summary>
/// Тесты для <see cref="ChatSession"/>.
/// </summary>
public class ChatSessionTests
{
    private static readonly LinkInfo[] NoLinks = Array.Empty<LinkInfo>();
    private static readonly LinkInfo[] WithLinks = { new("https://example.com", "Example") };

    private static Chat CreateTestChat()
    {
        var messages = new List<ChatMessage>
        {
            new TextMessage(1, new DateTime(2024, 1, 1), "Alice", "Hello", NoLinks, null),
            new MediaMessage(2, new DateTime(2024, 6, 15), "Bob", "Photo", NoLinks, null,
                MediaKind.Photo, "photo.jpg", true),
            new TextMessage(3, new DateTime(2024, 12, 31), "Alice", "Check this", WithLinks, null),
        };
        return new Chat("TestGroup", "group", 123L, messages);
    }

    [Fact]
    public void InitialState_ShouldBeEmpty()
    {
        var session = new ChatSession();
        Assert.Null(session.CurrentChat);
        Assert.Empty(session.FilteredMessages);
        Assert.Empty(session.Media);
        Assert.Empty(session.Links);
        Assert.Equal(MessageFilter.Empty, session.Filter);
    }

    [Fact]
    public void Load_ShouldSetCurrentChatAndCollections()
    {
        var session = new ChatSession();
        var chat = CreateTestChat();
        session.Load(chat);

        Assert.Same(chat, session.CurrentChat);
        Assert.Equal(3, session.FilteredMessages.Count);
        Assert.Single(session.Media);       // только MediaMessage
        Assert.Single(session.Links);       // только сообщение с ссылкой
    }

    [Fact]
    public void Load_ShouldRaiseChatLoadedEvent()
    {
        var session = new ChatSession();
        var chat = CreateTestChat();
        ChatLoadedEventArgs? raisedArgs = null;

        session.ChatLoaded += (sender, args) => raisedArgs = args;
        session.Load(chat);

        Assert.NotNull(raisedArgs);
        Assert.Same(chat, raisedArgs!.Chat);
    }

    [Fact]
    public void ApplyFilter_ShouldFilterMessages()
    {
        var session = new ChatSession();
        session.Load(CreateTestChat());

        var filter = new MessageFilter(null, "Bob", null, null, false, false);
        session.ApplyFilter(filter);

        Assert.Single(session.FilteredMessages);
        Assert.Equal("Bob", session.FilteredMessages[0].Author);
    }

    [Fact]
    public void ApplyFilter_ShouldRaiseFilterChangedEvent()
    {
        var session = new ChatSession();
        session.Load(CreateTestChat());
        var raised = false;

        session.FilterChanged += (_, _) => raised = true;
        session.ApplyFilter(MessageFilter.Empty);

        Assert.True(raised);
    }

    [Fact]
    public void ApplyFilter_WithOnlyMedia_ShouldReturnOnlyMediaMessages()
    {
        var session = new ChatSession();
        session.Load(CreateTestChat());

        var filter = new MessageFilter(null, null, null, null, true, false);
        session.ApplyFilter(filter);

        Assert.Single(session.FilteredMessages);
        Assert.IsType<MediaMessage>(session.FilteredMessages[0]);
    }

    [Fact]
    public void Clear_ShouldResetEverything()
    {
        var session = new ChatSession();
        session.Load(CreateTestChat());
        var raised = false;

        session.FilterChanged += (_, _) => raised = true;
        session.Clear();

        Assert.Null(session.CurrentChat);
        Assert.Empty(session.FilteredMessages);
        Assert.Empty(session.Media);
        Assert.Empty(session.Links);
        Assert.Equal(MessageFilter.Empty, session.Filter);
        Assert.True(raised);
    }

    [Fact]
    public void Media_ShouldNotBeAffectedByFilter()
    {
        var session = new ChatSession();
        session.Load(CreateTestChat());

        // Применяем фильтр, который исключает все сообщения
        session.ApplyFilter(new MessageFilter("NONEXISTENT", null, null, null, false, false));

        // Media должно остаться полным
        Assert.Single(session.Media);
        Assert.Empty(session.FilteredMessages);
    }

    [Fact]
    public void Links_ShouldNotBeAffectedByFilter()
    {
        var session = new ChatSession();
        session.Load(CreateTestChat());

        session.ApplyFilter(new MessageFilter("NONEXISTENT", null, null, null, false, false));

        // Links должен остаться полным
        Assert.Single(session.Links);
        Assert.Empty(session.FilteredMessages);
    }

    [Fact]
    public void LinkEntry_ShouldPairMessageAndLink()
    {
        var msg = new TextMessage(1, DateTime.Now, "Alice", "Link", WithLinks, null);
        var entry = new LinkEntry(msg, WithLinks[0]);

        Assert.Same(msg, entry.Message);
        Assert.Same(WithLinks[0], entry.Link);
    }
}