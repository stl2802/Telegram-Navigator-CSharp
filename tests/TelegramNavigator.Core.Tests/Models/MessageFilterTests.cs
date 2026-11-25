using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Tests.Models;

/// <summary>
/// Тесты для <see cref="MessageFilter"/>.
/// </summary>
public class MessageFilterTests
{
    private static readonly LinkInfo[] NoLinks = Array.Empty<LinkInfo>();
    private static readonly LinkInfo[] WithLinks = { new("https://example.com", "Example") };

    private static ChatMessage TextMsg => new TextMessage(1, new DateTime(2024, 6, 15, 12, 0, 0),
        "Alice", "Hello world", NoLinks, null);

    private static ChatMessage MediaMsg => new MediaMessage(2, new DateTime(2024, 6, 16, 14, 0, 0),
        "Bob", "Photo", NoLinks, null, MediaKind.Photo, null, false);

    private static ChatMessage MsgWithLinks => new TextMessage(3, new DateTime(2024, 6, 17, 10, 0, 0),
        "Charlie", "Check this https://example.com", WithLinks, null);

    [Fact]
    public void EmptyFilter_ShouldMatchAll()
    {
        var filter = MessageFilter.Empty;
        Assert.True(filter.Matches(TextMsg));
        Assert.True(filter.Matches(MediaMsg));
        Assert.True(filter.Matches(MsgWithLinks));
    }

    [Fact]
    public void SearchText_ShouldMatchCaseInsensitive()
    {
        var filter = new MessageFilter("hello", null, null, null, false, false);
        Assert.True(filter.Matches(TextMsg));
    }

    [Fact]
    public void SearchText_ShouldRespectCase()
    {
        var filter = new MessageFilter("WORLD", null, null, null, false, false);
        Assert.True(filter.Matches(TextMsg));
    }

    [Fact]
    public void SearchText_ShouldReturnFalseIfNoMatch()
    {
        var filter = new MessageFilter("goodbye", null, null, null, false, false);
        Assert.False(filter.Matches(TextMsg));
    }

[Theory]
    [InlineData("hello")]
    [InlineData("HELLO")]
    [InlineData("Hello")]
    public void SearchText_Theory_ShouldMatchCaseInsensitive(string search)
    {
        var filter = new MessageFilter(search, null, null, null, false, false);
        Assert.True(filter.Matches(TextMsg));
    }
    [Fact]
    public void SearchText_ShouldReturnFalseForEmptyText()
    {
        var filter = new MessageFilter("hello", null, null, null, false, false);
        var emptyMsg = new TextMessage(4, DateTime.Now, "Alice", "", NoLinks, null);
        Assert.False(filter.Matches(emptyMsg));
    }

    [Fact]
    public void Author_ShouldMatchCaseInsensitive()
    {
        var filter = new MessageFilter(null, "ALICE", null, null, false, false);
        Assert.True(filter.Matches(TextMsg));
    }

    [Fact]
    public void Author_ShouldReturnFalseIfNoMatch()
    {
        var filter = new MessageFilter(null, "David", null, null, false, false);
        Assert.False(filter.Matches(TextMsg));
    }

    [Fact]
    public void Author_ShouldReturnFalseForEmptyAuthor()
    {
        var filter = new MessageFilter(null, "Alice", null, null, false, false);
        var emptyAuthor = new TextMessage(5, DateTime.Now, "", "Text", NoLinks, null);
        Assert.False(filter.Matches(emptyAuthor));
    }

    [Fact]
    public void From_ShouldFilterInclusiveLowest()
    {
        var filter = new MessageFilter(null, null, new DateOnly(2024, 6, 15), null, false, false);
        Assert.True(filter.Matches(TextMsg));   // 15 июня
        Assert.True(filter.Matches(MediaMsg));  // 16 июня > 15 июня
    }

    [Fact]
    public void From_ShouldExcludeEarlier()
    {
        var filter = new MessageFilter(null, null, new DateOnly(2024, 6, 16), null, false, false);
        Assert.False(filter.Matches(TextMsg));  // 15 июня < 16 июня
    }

    [Fact]
    public void To_ShouldFilterInclusiveHighest()
    {
        var filter = new MessageFilter(null, null, null, new DateOnly(2024, 6, 16), false, false);
        Assert.True(filter.Matches(TextMsg));  // 15 июня <= 16 июня
        Assert.True(filter.Matches(MediaMsg)); // 16 июня <= 16 июня
    }

    [Fact]
    public void To_ShouldExcludeLater()
    {
        var filter = new MessageFilter(null, null, null, new DateOnly(2024, 6, 15), false, false);
        Assert.False(filter.Matches(MediaMsg)); // 16 июня > 15 июня
    }

    [Fact]
    public void OnlyWithMedia_ShouldFilter()
    {
        var filter = new MessageFilter(null, null, null, null, true, false);
        Assert.False(filter.Matches(TextMsg));
        Assert.True(filter.Matches(MediaMsg));
    }

    [Fact]
    public void OnlyWithLinks_ShouldFilter()
    {
        var filter = new MessageFilter(null, null, null, null, false, true);
        Assert.False(filter.Matches(TextMsg));
        Assert.True(filter.Matches(MsgWithLinks));
    }

    [Fact]
    public void MultipleConditions_ShouldCombineWithAnd()
    {
        // Alice, с медиа — не должно совпасть (у Alice нет медиа)
        var filter = new MessageFilter(null, "Alice", null, null, true, false);
        Assert.False(filter.Matches(TextMsg));
    }
}