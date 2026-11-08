using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Tests.Models;

/// <summary>
/// Тесты для иерархии <see cref="ChatMessage"/> и его наследников.
/// </summary>
public class ChatMessageTests
{
    private static readonly LinkInfo[] NoLinks = Array.Empty<LinkInfo>();

    [Fact]
    public void TextMessage_Preview_ShouldReturnFullTextIfShort()
    {
        var msg = new TextMessage(1, DateTime.Now, "Alice", "Hello!", NoLinks, null);
        Assert.Equal("Hello!", msg.Preview);
    }

    [Fact]
    public void TextMessage_Preview_ShouldTruncateLongText()
    {
        var longText = new string('A', 200);
        var msg = new TextMessage(2, DateTime.Now, "Bob", longText, NoLinks, null);
        Assert.Equal(100, msg.Preview.Length);
        Assert.EndsWith("...", msg.Preview);
    }

    [Fact]
    public void TextMessage_HasMedia_ShouldBeFalse()
    {
        var msg = new TextMessage(1, DateTime.Now, "Alice", "Hello", NoLinks, null);
        Assert.False(msg.HasMedia);
    }

    [Fact]
    public void MediaMessage_Preview_ShouldShowPhoto()
    {
        var msg = new MediaMessage(3, DateTime.Now, "Alice", "", NoLinks, null,
            MediaKind.Photo, "photos/photo_1.jpg", true);
        Assert.Contains("[Фото", msg.Preview);
        Assert.Contains("photo_1.jpg", msg.Preview);
    }

    [Fact]
    public void MediaMessage_Preview_ShouldShowVideo()
    {
        var msg = new MediaMessage(4, DateTime.Now, "Bob", "", NoLinks, null,
            MediaKind.Video, "videos/video_1.mp4", true);
        Assert.Contains("[Видео", msg.Preview);
        Assert.Contains("video_1.mp4", msg.Preview);
    }

    [Fact]
    public void MediaMessage_Preview_WithoutFileName_ShouldShowKindOnly()
    {
        var msg = new MediaMessage(5, DateTime.Now, "Alice", "", NoLinks, null,
            MediaKind.Sticker, null, false);
        Assert.Equal("[Стикер]", msg.Preview);
    }

    [Fact]
    public void MediaMessage_HasMedia_ShouldBeTrue()
    {
        var msg = new MediaMessage(6, DateTime.Now, "Alice", "", NoLinks, null,
            MediaKind.Photo, null, false);
        Assert.True(msg.HasMedia);
    }

    [Theory]
    [InlineData(MediaKind.VideoMessage, "Кружок")]
    [InlineData(MediaKind.VoiceMessage, "Голосовое")]
    [InlineData(MediaKind.Audio, "Аудио")]
    [InlineData(MediaKind.Animation, "GIF")]
    [InlineData(MediaKind.Document, "Документ")]
    [InlineData(MediaKind.None, "Медиа")]
    [InlineData(MediaKind.Photo, "Фото")]
    public void MediaMessage_Preview_ShouldProduceCorrectKindLabels(MediaKind kind, string expectedLabel)
    {
        var msg = new MediaMessage(7, DateTime.Now, "Alice", "", NoLinks, null,
            kind, null, false);
        Assert.Contains(expectedLabel, msg.Preview);
    }

    [Fact]
    public void ServiceMessage_Preview_ShouldShowAction()
    {
        var msg = new ServiceMessage(8, DateTime.Now, "System", "User joined", NoLinks, null, "join");
        Assert.Equal("[Служебное: join]", msg.Preview);
    }

    [Fact]
    public void ServiceMessage_HasMedia_ShouldBeFalse()
    {
        var msg = new ServiceMessage(9, DateTime.Now, "System", "Action", NoLinks, null, "action");
        Assert.False(msg.HasMedia);
    }

    [Fact]
    public void ChatMessage_ShouldSupportWithOperator()
    {
        var original = new TextMessage(10, DateTime.Now, "Alice", "Original", NoLinks, null);
        var modified = original with { Text = "Modified" };
        Assert.Equal("Modified", modified.Text);
        Assert.Equal("Original", original.Text);
        Assert.NotEqual(original, modified);
    }

    [Fact]
    public void TextMessage_ShouldBeSealed()
    {
        Assert.True(typeof(TextMessage).IsSealed);
    }

    [Fact]
    public void MediaMessage_ShouldBeSealed()
    {
        Assert.True(typeof(MediaMessage).IsSealed);
    }

    [Fact]
    public void ServiceMessage_ShouldBeSealed()
    {
        Assert.True(typeof(ServiceMessage).IsSealed);
    }
}