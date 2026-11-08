using System.Text.Json;
using TelegramNavigator.Core.Models;
using TelegramNavigator.Core.Parsing;
using TelegramNavigator.Core.Parsing.Dto;

namespace TelegramNavigator.Core.Tests.Parsing;

/// <summary>
/// Тесты для <see cref="TelegramMessageMapper"/>.
/// </summary>
public class TelegramMessageMapperTests
{
    [Fact]
    public void Map_TextMessage_ShouldReturnTextMessage()
    {
        var dto = new TelegramMessageDto
        {
            Id = 1, Type = "message",
            Date = "2024-06-15T10:30:00",
            From = "Alice",
            Text = JsonDocument.Parse("\"Hello!\"").RootElement,
        };

        var result = TelegramMessageMapper.Map(dto, null);
        Assert.IsType<TextMessage>(result);
        var msg = (TextMessage)result!;
        Assert.Equal(1, msg.Id);
        Assert.Equal("Alice", msg.Author);
        Assert.Equal("Hello!", msg.Text);
        Assert.Empty(msg.Links);
        Assert.Null(msg.ReplyToId);
    }

    [Fact]
    public void Map_ServiceMessage_ShouldReturnServiceMessage()
    {
        var dto = new TelegramMessageDto
        {
            Id = 2, Type = "service",
            Date = "2024-06-15T10:30:00",
            Actor = "Charlie",
            Action = "pin",
            Text = JsonDocument.Parse("\"Charlie pinned\"").RootElement,
        };

        var result = TelegramMessageMapper.Map(dto, null);
        Assert.IsType<ServiceMessage>(result);
        var msg = (ServiceMessage)result!;
        Assert.Equal("Charlie", msg.Author);
        Assert.Equal("pin", msg.Action);
    }

    [Fact]
    public void Map_PhotoMessage_ShouldReturnMediaMessageWithPhoto()
    {
        var dto = new TelegramMessageDto
        {
            Id = 3, Type = "message",
            Date = "2024-06-15T10:30:00",
            From = "Alice",
            Photo = "photos/photo_1.jpg",
            Text = JsonDocument.Parse("\"Photo\"").RootElement,
        };

        var result = TelegramMessageMapper.Map(dto, null);
        Assert.IsType<MediaMessage>(result);
        var msg = (MediaMessage)result!;
        Assert.Equal(MediaKind.Photo, msg.Kind);
    }

    [Theory]
    [InlineData("video_file", MediaKind.Video)]
    [InlineData("voice_message", MediaKind.VoiceMessage)]
    [InlineData("video_message", MediaKind.VideoMessage)]
    [InlineData("audio_file", MediaKind.Audio)]
    [InlineData("sticker", MediaKind.Sticker)]
    [InlineData("animation", MediaKind.Animation)]
    public void Map_FileMessage_ShouldMapMediaKind(string mediaType, MediaKind expected)
    {
        var dto = new TelegramMessageDto
        {
            Id = 4, Type = "message",
            Date = "2024-06-15T10:30:00",
            From = "Bob",
            File = "file.bin",
            MediaType = mediaType,
            Text = JsonDocument.Parse("\"\"").RootElement,
        };
        var result = TelegramMessageMapper.Map(dto, null);
        Assert.IsType<MediaMessage>(result);
        var msg = (MediaMessage)result!;
        Assert.Equal(expected, msg.Kind);
    }

    [Fact]
    public void Map_FileWithoutMediaType_ShouldDefaultToDocument()
    {
        var dto = new TelegramMessageDto
        {
            Id = 5, Type = "message",
            Date = "2024-06-15T10:30:00",
            From = "Bob", File = "doc.pdf",
            MediaType = null,
            Text = JsonDocument.Parse("\"\"").RootElement,
        };
        var result = TelegramMessageMapper.Map(dto, null);
        var msg = (MediaMessage)result!;
        Assert.Equal(MediaKind.Document, msg.Kind);
    }

    [Fact]
    public void Map_NullFromAndActor_ShouldSetAuthorUnknown()
    {
        var dto = new TelegramMessageDto
        {
            Id = 6, Type = "message",
            Date = "2024-06-15T10:30:00",
            From = null, Actor = null,
            Text = JsonDocument.Parse("\"Text\"").RootElement,
        };
        var result = TelegramMessageMapper.Map(dto, null);
        Assert.NotNull(result);
        Assert.Equal("Неизвестный", result!.Author);
    }

    [Fact]
    public void Map_FileNotIncluded_ShouldSetIsFileAvailableFalse()
    {
        var dto = new TelegramMessageDto
        {
            Id = 7, Type = "message",
            Date = "2024-06-15T10:30:00",
            From = "Alice",
            File = "(File not included. Change data exporting settings to download.)",
            MediaType = "video_file",
            Text = JsonDocument.Parse("\"\"").RootElement,
        };
        var result = TelegramMessageMapper.Map(dto, "/base");
        var msg = (MediaMessage)result!;
        Assert.False(msg.IsFileAvailable);
    }

    [Fact]
    public void Map_WithTextEntities_ShouldExtractTwoLinks()
    {
        var dto = new TelegramMessageDto
        {
            Id = 8, Type = "message",
            Date = "2024-06-15T10:30:00",
            From = "Alice",
            TextEntities = new List<TextEntityDto>
            {
                new() { Type = "link", Text = "https://ex.com" },
                new() { Type = "text_link", Text = "Ex", Href = "https://ex.net" },
            },
            Text = JsonDocument.Parse("\"L\"").RootElement,
        };
        var result = TelegramMessageMapper.Map(dto, null);
        Assert.Equal(2, result!.Links.Count);
        Assert.Equal("https://ex.com", result.Links[0].Url);
        Assert.Null(result.Links[0].Title);
        Assert.Equal("https://ex.net", result.Links[1].Url);
        Assert.Equal("Ex", result.Links[1].Title);
    }

    [Fact]
    public void Map_WithReplyTo_ShouldSetReplyToId()
    {
        var dto = new TelegramMessageDto
        {
            Id = 9, Type = "message",
            Date = "2024-06-15T10:30:00",
            From = "Alice",
            ReplyToMessageId = 1,
            Text = JsonDocument.Parse("\"R\"").RootElement,
        };
        var result = TelegramMessageMapper.Map(dto, null);
        Assert.Equal(1, result!.ReplyToId);
    }

    [Fact]
    public void ExtractText_FromTextEntities_ShouldConcatenate()
    {
        var dto = new TelegramMessageDto
        {
            TextEntities = new List<TextEntityDto>
            {
                new() { Type = "plain", Text = "Hello " },
                new() { Type = "bold", Text = "World" },
            },
        };
        Assert.Equal("Hello World", TelegramMessageMapper.ExtractText(dto));
    }

    [Fact]
    public void ExtractText_FromStringElement_ShouldReturnString()
    {
        var dto = new TelegramMessageDto
        {
            Text = JsonDocument.Parse("\"Hello\"").RootElement,
        };
        Assert.Equal("Hello", TelegramMessageMapper.ExtractText(dto));
    }

    [Fact]
    public void ExtractText_FromArrayElement_ShouldConcatenate()
    {
        var json = "[\"Hello \", {\"type\":\"mention\",\"text\":\"@Bob\"}, \"!\"]";
        var dto = new TelegramMessageDto
        {
            Text = JsonDocument.Parse(json).RootElement,
        };
        Assert.Equal("Hello @Bob!", TelegramMessageMapper.ExtractText(dto));
    }

    [Fact]
    public void ParseDate_FromIsoString_ShouldReturnDateTime()
    {
        var date = TelegramMessageMapper.ParseDate("2024-06-15T10:30:00", "");
        Assert.Equal(new DateTime(2024, 6, 15, 10, 30, 0), date);
    }

    [Fact]
    public void ParseDate_FromUnixTime_ShouldReturnDateTime()
    {
        // 1718447400 = 2024-06-15T10:30:00 UTC
        var date = TelegramMessageMapper.ParseDate("", "1718447400");
        Assert.Equal(new DateTime(2024, 6, 15, 10, 30, 0), date);
    }

    [Fact]
    public void ParseDate_InvalidInput_ShouldReturnNull()
    {
        Assert.Null(TelegramMessageMapper.ParseDate("bad", "also bad"));
    }

    [Fact]
    public void MapMediaKind_NullString_ShouldReturnDocument()
    {
        Assert.Equal(MediaKind.Document, TelegramMessageMapper.MapMediaKind(null));
    }
}