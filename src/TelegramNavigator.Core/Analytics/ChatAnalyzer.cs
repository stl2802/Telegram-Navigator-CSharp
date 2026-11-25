using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Analytics;

/// <summary>
/// Аналитика чата на основе LINQ-запросов.
/// </summary>
public class ChatAnalyzer : IChatAnalyzer
{
    /// <summary>
    /// Возвращает топ-N авторов по количеству сообщений.
    /// ServiceMessage не учитываются.
    /// </summary>
    public IReadOnlyList<UserStat> GetTopAuthors(IEnumerable<ChatMessage> messages, int count)
    {
        var allMessages = messages.ToList();
        var total = allMessages.Count;

        var authors = allMessages
            .Where(m => m is not ServiceMessage)
            .GroupBy(m => m.Author)
            .Select(g => new UserStat(
                g.Key,
                g.Count(),
                total > 0 ? Math.Round((double)g.Count() / total * 100, 1) : 0.0))
            .OrderByDescending(s => s.MessageCount)
            .Take(count)
            .ToList();

        return authors;
    }

    /// <summary>
    /// Активность по дням: количество сообщений за каждый день.
    /// Дни без сообщений не включаются. Сортировка по дате.
    /// </summary>
    public IReadOnlyList<DailyActivity> GetDailyActivity(IEnumerable<ChatMessage> messages)
    {
        return messages
            .GroupBy(m => DateOnly.FromDateTime(m.Date))
            .Select(g => new DailyActivity(g.Key, g.Count()))
            .OrderBy(d => d.Day)
            .ToList();
    }

    /// <summary>
    /// Активность по часам: суммарное количество сообщений за каждый час (0–23).
    /// </summary>
    public IReadOnlyList<HourlyActivity> GetHourlyActivity(IEnumerable<ChatMessage> messages)
    {
        return messages
            .GroupBy(m => m.Date.Hour)
            .Select(g => new HourlyActivity(g.Key, g.Count()))
            .OrderBy(h => h.Hour)
            .ToList();
    }

    /// <summary>
    /// Общая сводка по чату.
    /// </summary>
    public ChatSummary GetSummary(IEnumerable<ChatMessage> messages)
    {
        var allMessages = messages.ToList();

        if (allMessages.Count == 0)
        {
            return new ChatSummary(0, 0, 0, 0, null, null);
        }

        var participants = allMessages
            .Select(m => m.Author)
            .Where(a => !string.IsNullOrEmpty(a))
            .Distinct()
            .Count();

        var mediaCount = allMessages.Count(m => m is MediaMessage);
        var linkCount = allMessages.Sum(m => m.Links.Count);
        var first = allMessages.Min(m => m.Date);
        var last = allMessages.Max(m => m.Date);

        return new ChatSummary(
            allMessages.Count,
            participants,
            mediaCount,
            linkCount,
            first,
            last);
    }

    /// <summary>
    /// Возвращает топ пар «кто → кому ответил».
    /// </summary>
    public IReadOnlyList<ReplyEdge> GetReplyPairs(IReadOnlyList<ChatMessage> messages, int topCount)
    {
        // Словарь: Id сообщения → автор (только непустые авторы)
        var idToAuthor = messages
            .Where(m => !string.IsNullOrEmpty(m.Author))
            .ToDictionary(m => m.Id, m => m.Author);

        // Собираем пары: (кто ответил, кому ответили)
        var pairs = messages
            .Where(m => m is not ServiceMessage)
            .Where(m => m.ReplyToId.HasValue && idToAuthor.ContainsKey(m.ReplyToId.Value))
            .Select(m => (From: m.Author, To: idToAuthor[m.ReplyToId!.Value]))
            .Where(x => x.From != x.To)
            .GroupBy(x => (x.From, x.To))
            .Select(g => new ReplyEdge(g.Key.From, g.Key.To, g.Count()))
            .OrderByDescending(e => e.Count)
            .Take(topCount)
            .ToList();

        return pairs;
    }
}