using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Tests.Models;

/// <summary>
/// Тесты для <see cref="ChatParseException"/>.
/// </summary>
public class ChatParseExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var ex = new ChatParseException("Parsing error");
        Assert.Equal("Parsing error", ex.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndInner_ShouldSetBoth()
    {
        var inner = new InvalidOperationException("Inner");
        var ex = new ChatParseException("Parsing error", inner);
        Assert.Equal("Parsing error", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void Exception_ShouldBeCatchableAsException()
    {
        var ex = new ChatParseException("Error");
        Assert.IsAssignableFrom<Exception>(ex);
    }
}