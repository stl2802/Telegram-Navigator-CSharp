using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Tests.Models;

/// <summary>
/// Тесты для enum <see cref="MediaKind"/>.
/// </summary>
public class MediaKindTests
{
    [Fact]
    public void MediaKind_ShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)MediaKind.None);
        Assert.Equal(1, (int)MediaKind.Photo);
        Assert.Equal(2, (int)MediaKind.Video);
        Assert.Equal(3, (int)MediaKind.VideoMessage);
        Assert.Equal(4, (int)MediaKind.VoiceMessage);
        Assert.Equal(5, (int)MediaKind.Audio);
        Assert.Equal(6, (int)MediaKind.Sticker);
        Assert.Equal(7, (int)MediaKind.Animation);
        Assert.Equal(8, (int)MediaKind.Document);
    }

    [Fact]
    public void MediaKind_ShouldHaveNineMembers()
    {
        var names = Enum.GetNames<MediaKind>();
        Assert.Equal(9, names.Length);
    }
}