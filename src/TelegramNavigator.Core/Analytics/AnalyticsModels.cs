namespace TelegramNavigator.Core.Analytics;

/// <summary>
/// Статистика одного автора.
/// </summary>
/// <param name="Author">Имя автора.</param>
/// <param name="MessageCount">Количество сообщений.</param>
/// <param name="Percent">Доля от общего числа сообщений (0–100).</param>
public record UserStat(string Author, int MessageCount, double Percent);

/// <summary>
/// Активность за один день.
/// </summary>
/// <param name="Day">Дата.</param>
/// <param name="Count">Количество сообщений.</param>
public record DailyActivity(DateOnly Day, int Count);

/// <summary>
/// Активность в один час (0–23).
/// </summary>
/// <param name="Hour">Час (0–23).</param>
/// <param name="Count">Суммарное количество сообщений за этот час.</param>
public record HourlyActivity(int Hour, int Count);

/// <summary>
/// Сводная статистика по чату.
/// </summary>
/// <param name="TotalMessages">Общее количество сообщений.</param>
/// <param name="Participants">Количество участников.</param>
/// <param name="MediaCount">Количество сообщений с медиа.</param>
/// <param name="LinkCount">Количество ссылок.</param>
/// <param name="First">Дата первого сообщения (null, если сообщений нет).</param>
/// <param name="Last">Дата последнего сообщения (null, если сообщений нет).</param>
public record ChatSummary(
    int TotalMessages,
    int Participants,
    int MediaCount,
    int LinkCount,
    DateTime? First,
    DateTime? Last);