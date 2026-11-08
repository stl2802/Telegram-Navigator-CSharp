using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Export;
using TelegramNavigator.Core.Models;
using TelegramNavigator.Core.Parsing;
using TelegramNavigator.Core.Services;
using TelegramNavigator.Core.Settings;

namespace TelegramNavigator.Core.Tests.Models;

/// <summary>
/// Проверка, что все интерфейсы этапа 1 существуют и компилируются.
/// </summary>
public class InterfaceContractsTests
{
    [Fact]
    public void IChatParser_ShouldBeInterface()
    {
        Assert.True(typeof(IChatParser).IsInterface);
    }

    [Fact]
    public void IChatAnalyzer_ShouldBeInterface()
    {
        Assert.True(typeof(IChatAnalyzer).IsInterface);
    }

    [Fact]
    public void IChatExporter_ShouldBeInterface()
    {
        Assert.True(typeof(IChatExporter).IsInterface);
    }

    [Fact]
    public void ISettingsService_ShouldBeInterface()
    {
        Assert.True(typeof(ISettingsService).IsInterface);
    }

    [Fact]
    public void ChatLoadedEventArgs_ShouldInheritEventArgs()
    {
        Assert.IsAssignableFrom<EventArgs>(new ChatLoadedEventArgs(
            new Chat("Test", "type", 1L, Array.Empty<ChatMessage>())));
    }
}