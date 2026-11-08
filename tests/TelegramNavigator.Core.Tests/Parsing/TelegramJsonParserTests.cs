using TelegramNavigator.Core.Models;
using TelegramNavigator.Core.Parsing;

namespace TelegramNavigator.Core.Tests.Parsing;

/// <summary>
/// Тесты для <see cref="TelegramJsonParser"/> с реальным JSON.
/// </summary>
public class TelegramJsonParserTests
{
    private readonly TelegramJsonParser _parser = new();

    private static string GetTestDataPath()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "TestData")))
            dir = Path.GetDirectoryName(dir);
        return Path.Combine(dir!, "TestData", "sample_result.json");
    }

    [Fact]
    public async Task ParseFileAsync_ShouldReturnChatWithCorrectMetadata()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());

        Assert.Equal("Тестовая группа", chat.Name);
        Assert.Equal("private_group", chat.Type);
        Assert.Equal(12345, chat.Id);
    }

    [Fact]
    public async Task ParseFileAsync_ShouldParseAllMessages()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        Assert.Equal(18, chat.Messages.Count);
    }

    [Fact]
    public async Task ParseFileAsync_ShouldHaveThreeTextMessages()
    {
        // 18 total - 10 media - 2 service = 6 text messages
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        var texts = chat.Messages.OfType<TextMessage>().ToList();
        Assert.Equal(6, texts.Count);
    }

    [Fact]
    public async Task ParseFileAsync_ShouldHaveTwoServiceMessages()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        var services = chat.Messages.OfType<ServiceMessage>().ToList();
        Assert.Equal(2, services.Count);
        Assert.Contains(services, s => s.Action == "pin");
        Assert.Contains(services, s => s.Action == "add_user");
    }

    [Fact]
    public async Task ParseFileAsync_ShouldParseMediaMessages()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        var media = chat.Messages.OfType<MediaMessage>().ToList();
        Assert.Equal(10, media.Count);
    }

    [Fact]
    public async Task ParseFileAsync_ShouldDetectMessageWithUnavailableFile()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        var unavailable = chat.Messages
            .OfType<MediaMessage>()
            .SingleOrDefault(m => m.Id == 7);
        Assert.NotNull(unavailable);
        Assert.False(unavailable.IsFileAvailable);
    }

    [Fact]
    public async Task ParseFileAsync_ShouldParseReplyTo()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        var replyMsg = chat.Messages.SingleOrDefault(m => m.Id == 3);
        Assert.NotNull(replyMsg);
        Assert.Equal(1, replyMsg.ReplyToId);
    }

    [Fact]
    public async Task ParseFileAsync_ShouldParseMessagesWithNullFrom()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        var nullFrom = chat.Messages.SingleOrDefault(m => m.Id == 9);
        Assert.NotNull(nullFrom);
        Assert.Equal("Неизвестный", nullFrom.Author);
    }

    [Fact]
    public async Task ParseFileAsync_ShouldParseMessageWithTextArray()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        var msg = chat.Messages.SingleOrDefault(m => m.Id == 3);
        Assert.NotNull(msg);
        Assert.Contains("@Боб", msg.Text);
    }

    [Fact]
    public async Task ParseFileAsync_ShouldExtractLinksFromTextEntities()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        var msg = chat.Messages.SingleOrDefault(m => m.Id == 2);
        Assert.NotNull(msg);
        Assert.Equal(2, msg.Links.Count);
        Assert.Equal("GitHub", msg.Links[0].Title);
        Assert.Equal("https://github.com", msg.Links[0].Url);
        Assert.Equal("https://example.com", msg.Links[1].Url);
    }

    [Fact]
    public async Task ParseFileAsync_Chat_ShouldCalculateParticipants()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        // Алиса, Боб, Чарли + "Неизвестный"
        Assert.Equal(4, chat.Participants);
    }

    [Fact]
    public async Task ParseFileAsync_Chat_ShouldCalculateFirstAndLastDate()
    {
        var chat = await _parser.ParseFileAsync(GetTestDataPath());
        Assert.Equal(new DateTime(2024, 6, 1, 10, 30, 0), chat.FirstDate);
        Assert.Equal(new DateTime(2024, 6, 4, 21, 10, 0), chat.LastDate);
    }

    [Fact]
    public async Task ParseAsync_WithInvalidJson_ShouldThrowChatParseException()
    {
        using var stream = new MemoryStream("not json"u8.ToArray());
        await Assert.ThrowsAsync<ChatParseException>(
            () => _parser.ParseAsync(stream, null));
    }

    [Fact]
    public async Task ParseFileAsync_NonExistentFile_ShouldThrowChatParseException()
    {
        await Assert.ThrowsAsync<ChatParseException>(
            () => _parser.ParseFileAsync("C:/nonexistent/file.json"));
    }
}