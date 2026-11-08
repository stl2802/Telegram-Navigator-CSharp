using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Tests.Models;

/// <summary>
/// Тесты для record <see cref="LinkInfo"/>.
/// </summary>
public class LinkInfoTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var link = new LinkInfo("https://example.com", "Example");
        Assert.Equal("https://example.com", link.Url);
        Assert.Equal("Example", link.Title);
    }

    [Fact]
    public void Constructor_ShouldAllowNullTitle()
    {
        var link = new LinkInfo("https://example.com", null);
        Assert.Equal("https://example.com", link.Url);
        Assert.Null(link.Title);
    }

    [Fact]
    public void Equality_ShouldBeByValue()
    {
        var a = new LinkInfo("https://example.com", "Example");
        var b = new LinkInfo("https://example.com", "Example");
        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Inequality_ShouldDetectDifferences()
    {
        var a = new LinkInfo("https://example.com", "Example");
        var b = new LinkInfo("https://other.com", "Other");
        Assert.NotEqual(a, b);
    }
}