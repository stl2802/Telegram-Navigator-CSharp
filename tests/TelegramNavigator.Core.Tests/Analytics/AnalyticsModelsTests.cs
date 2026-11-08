using TelegramNavigator.Core.Analytics;

namespace TelegramNavigator.Core.Tests.Analytics;

/// <summary>
/// Тесты для моделей аналитики.
/// </summary>
public class AnalyticsModelsTests
{
    [Fact]
    public void UserStat_ShouldSetProperties()
    {
        var stat = new UserStat("Alice", 10, 25.0);
        Assert.Equal("Alice", stat.Author);
        Assert.Equal(10, stat.MessageCount);
        Assert.Equal(25.0, stat.Percent);
    }

    [Fact]
    public void DailyActivity_ShouldSetProperties()
    {
        var activity = new DailyActivity(new DateOnly(2024, 6, 15), 42);
        Assert.Equal(new DateOnly(2024, 6, 15), activity.Day);
        Assert.Equal(42, activity.Count);
    }

    [Fact]
    public void HourlyActivity_ShouldSetProperties()
    {
        var activity = new HourlyActivity(14, 100);
        Assert.Equal(14, activity.Hour);
        Assert.Equal(100, activity.Count);
    }

    [Fact]
    public void ChatSummary_ShouldSetProperties()
    {
        var first = new DateTime(2024, 1, 1);
        var last = new DateTime(2024, 12, 31);
        var summary = new ChatSummary(1000, 15, 200, 50, first, last);

        Assert.Equal(1000, summary.TotalMessages);
        Assert.Equal(15, summary.Participants);
        Assert.Equal(200, summary.MediaCount);
        Assert.Equal(50, summary.LinkCount);
        Assert.Equal(first, summary.First);
        Assert.Equal(last, summary.Last);
    }

    [Fact]
    public void ChatSummary_FirstAndLast_CanBeNull()
    {
        var summary = new ChatSummary(0, 0, 0, 0, null, null);
        Assert.Null(summary.First);
        Assert.Null(summary.Last);
    }

    [Fact]
    public void ChatSummary_Percent_ShouldBeDouble()
    {
        var stat = new UserStat("Bob", 3, 33.333);
        Assert.Equal(33.333, stat.Percent, precision: 3);
    }
}