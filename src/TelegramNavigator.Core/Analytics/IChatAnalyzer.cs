using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Core.Analytics;

/// <summary>
/// Аналитика чата: статистика по авторам, активность по дням/часам, сводка.
/// </summary>
public interface IChatAnalyzer
{
    /// <summary>
    /// Возвращает топ-N авторов по количеству сообщений.
    /// </summary>
    IReadOnlyList<UserStat> GetTopAuthors(IEnumerable<ChatMessage> messages, int count);

    /// <summary>
    /// Активность по дням (количество сообщений за каждый день).
    /// </summary>
    IReadOnlyList<DailyActivity> GetDailyActivity(IEnumerable<ChatMessage> messages);

    /// <summary>
    /// Активность по часам (суммарно за все дни).
    /// </summary>
    IReadOnlyList<HourlyActivity> GetHourlyActivity(IEnumerable<ChatMessage> messages);

    /// <summary>
    /// Общая сводка по чату.
    /// </summary>
    ChatSummary GetSummary(IEnumerable<ChatMessage> messages);
}