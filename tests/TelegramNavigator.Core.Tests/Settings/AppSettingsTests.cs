using TelegramNavigator.Core.Settings;

namespace TelegramNavigator.Core.Tests.Settings;

/// <summary>
/// Тесты для record <see cref="AppSettings"/>.
/// </summary>
public class AppSettingsTests
{
    [Fact]
    public void Default_ShouldUseLightTheme()
    {
        var defaults = AppSettings.Default;
        Assert.Equal("Light", defaults.Theme);
        Assert.Null(defaults.LastPath);
        Assert.Null(defaults.SearchText);
        Assert.Null(defaults.Author);
        Assert.False(defaults.OnlyWithMedia);
        Assert.False(defaults.OnlyWithLinks);
        Assert.Empty(defaults.RecentFiles);
    }

    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var recent = new List<string> { "file1.json", "file2.json" };
        var settings = new AppSettings("Dark", "/path/to/file", "hello", "Alice",
            true, true, recent);

        Assert.Equal("Dark", settings.Theme);
        Assert.Equal("/path/to/file", settings.LastPath);
        Assert.Equal("hello", settings.SearchText);
        Assert.Equal("Alice", settings.Author);
        Assert.True(settings.OnlyWithMedia);
        Assert.True(settings.OnlyWithLinks);
        Assert.Equal(2, settings.RecentFiles.Count);
    }

    [Fact]
    public void Equality_ShouldBeByValue()
    {
        var a = AppSettings.Default;
        var b = AppSettings.Default;
        Assert.Equal(a, b);
        Assert.True(a == b);
    }
}