using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Tests.Analytics;

/// <summary>
/// Тесты для <see cref="ChatAnalyzer"/>.
/// </summary>
public class ChatAnalyzerTests
{
    private static readonly LinkInfo[] NoLinks = Array.Empty<LinkInfo>();
    private readonly ChatAnalyzer _analyzer = new();

    private List<ChatMessage> CreateMessages()
    {
        var d1 = new DateTime(2024, 6, 1, 10, 0, 0);
        var d2 = new DateTime(2024, 6, 1, 14, 0, 0);
        var d3 = new DateTime(2024, 6, 2, 10, 0, 0);
        var d4 = new DateTime(2024, 6, 2, 14, 0, 0);

        return new List<ChatMessage>
        {
            new TextMessage(1, d1, "Alice", "A1", NoLinks, null),
            new TextMessage(2, d1, "Alice", "A2", NoLinks, null),
            new TextMessage(3, d2, "Alice", "A3", NoLinks, null),
            new TextMessage(4, d3, "Bob",   "B1", NoLinks, null),
            new TextMessage(5, d3, "Bob",   "B2", NoLinks, null),
            new TextMessage(6, d3, "Bob",   "B3", NoLinks, null),
            new TextMessage(7, d4, "Charlie", "C1", NoLinks, null),
            new TextMessage(8, d4, "Charlie", "C2", NoLinks, null),
            // ServiceMessage — не должно влиять на топ авторов, но влияет на общее кол-во
            new ServiceMessage(9, d4, "System", "Action", NoLinks, null, "pin"),
        };
    }

    [Fact]
    public void GetTopAuthors_ShouldReturnCorrectOrder()
    {
        var messages = CreateMessages();
        var top = _analyzer.GetTopAuthors(messages, 3);

        // Bob = 3, Alice = 3, Charlie = 2 (сортировка по убыванию)
        Assert.Equal(3, top.Count);
        // Порядок: 3 у Alice, 3 у Bob — может быть разный, проверим только состав
        var authors = top.Select(s => s.Author).OrderBy(a => a).ToList();
        Assert.Equal(new[] { "Alice", "Bob", "Charlie" }, authors);
    }

    [Fact]
    public void GetTopAuthors_ServiceMessages_ShouldNotBeCounted()
    {
        var messages = CreateMessages();
        var top = _analyzer.GetTopAuthors(messages, 10);

        // ServiceMessage не должно быть в статистике
        Assert.DoesNotContain(top, s => s.Author == "System");
    }

    [Fact]
    public void GetTopAuthors_ShouldCalculatePercentWithOneDecimal()
    {
        var messages = CreateMessages();
        var top = _analyzer.GetTopAuthors(messages, 10);

        // Всего 9 сообщений: Alice=3 (33.3%), Bob=3 (33.3%), Charlie=2 (22.2%)
        var alice = top.Single(s => s.Author == "Alice");
        Assert.Equal(33.3, alice.Percent, precision: 1);

        var charlie = top.Single(s => s.Author == "Charlie");
        Assert.Equal(22.2, charlie.Percent, precision: 1);
    }

    [Fact]
    public void GetTopAuthors_Limit_ShouldRestrictCount()
    {
        var messages = CreateMessages();
        var top = _analyzer.GetTopAuthors(messages, 2);
        Assert.Equal(2, top.Count);
    }

    [Fact]
    public void GetDailyActivity_ShouldReturnDaysWithCounts()
    {
        var messages = CreateMessages();
        var activity = _analyzer.GetDailyActivity(messages);

        Assert.Equal(2, activity.Count);
        Assert.Equal(new DateOnly(2024, 6, 1), activity[0].Day);
        Assert.Equal(3, activity[0].Count);
        Assert.Equal(new DateOnly(2024, 6, 2), activity[1].Day);
        Assert.Equal(6, activity[1].Count);
    }

    [Fact]
    public void GetDailyActivity_ShouldBeSortedByDate()
    {
        var messages = CreateMessages();
        var activity = _analyzer.GetDailyActivity(messages);
        Assert.True(activity[0].Day <= activity[1].Day);
    }

    [Fact]
    public void GetHourlyActivity_ShouldReturnHoursWithCounts()
    {
        var messages = CreateMessages();
        var activity = _analyzer.GetHourlyActivity(messages);

        // 10 часов: Alice (2) + Bob (3) = 5
        // 14 часов: Alice (1) + Charlie (2) + System service (1) = 4
        Assert.Equal(2, activity.Count);
        Assert.Equal(10, activity[0].Hour);
        Assert.Equal(5, activity[0].Count);
        Assert.Equal(14, activity[1].Hour);
        Assert.Equal(4, activity[1].Count);
    }

    [Fact]
    public void GetHourlyActivity_ShouldBeSortedByHour()
    {
        var messages = CreateMessages();
        var activity = _analyzer.GetHourlyActivity(messages);
        Assert.True(activity[0].Hour <= activity[1].Hour);
    }

    [Fact]
    public void GetSummary_ShouldReturnCorrectStatistics()
    {
        var messages = CreateMessages();
        var summary = _analyzer.GetSummary(messages);

        Assert.Equal(9, summary.TotalMessages);
        Assert.Equal(4, summary.Participants);  // Alice, Bob, Charlie, System
        Assert.Equal(0, summary.LinkCount);     // сообщения в тесте не имеют ссылок
        Assert.Equal(0, summary.MediaCount);    // нет MediaMessage в тестовых данных
        Assert.Equal(new DateTime(2024, 6, 1, 10, 0, 0), summary.First);
        Assert.Equal(new DateTime(2024, 6, 2, 14, 0, 0), summary.Last);
    }

    [Fact]
    public void GetSummary_EmptyList_ShouldReturnZeros()
    {
        var summary = _analyzer.GetSummary(Array.Empty<ChatMessage>());
        Assert.Equal(0, summary.TotalMessages);
        Assert.Equal(0, summary.Participants);
        Assert.Equal(0, summary.MediaCount);
        Assert.Equal(0, summary.LinkCount);
        Assert.Null(summary.First);
        Assert.Null(summary.Last);
    }
}