using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Desktop.ViewModels;

/// <summary>
/// ViewModel для вкладки аналитики.
/// </summary>
public partial class AnalyticsViewModel : ViewModelBase
{
    public ObservableCollection<BarItemViewModel> TopAuthors { get; } = new();
    public ObservableCollection<BarItemViewModel> DailyActivity { get; } = new();
    public ObservableCollection<BarItemViewModel> HourlyActivity { get; } = new();

    [ObservableProperty]
    private ChatSummary? _summary;

    /// <summary>
    /// Обновляет всю аналитику по переданным сообщениям.
    /// </summary>
    public void Update(IEnumerable<ChatMessage> messages, IChatAnalyzer analyzer)
    {
        var list = messages.ToList();

        // Топ авторов
        TopAuthors.Clear();
        var authors = analyzer.GetTopAuthors(list, 10);
        var maxAuthorCount = authors.Count > 0 ? authors.Max(a => a.MessageCount) : 1;
        foreach (var a in authors)
        {
            TopAuthors.Add(new BarItemViewModel(
                a.Author, a.MessageCount, (double)a.MessageCount / maxAuthorCount * 300));
        }

        // По дням
        DailyActivity.Clear();
        var days = analyzer.GetDailyActivity(list);
        var maxDayCount = days.Count > 0 ? days.Max(d => d.Count) : 1;
        foreach (var d in days)
        {
            DailyActivity.Add(new BarItemViewModel(
                d.Day.ToString("dd.MM"), d.Count, (double)d.Count / maxDayCount * 300));
        }

        // По часам
        HourlyActivity.Clear();
        var hours = analyzer.GetHourlyActivity(list);
        var maxHourCount = hours.Count > 0 ? hours.Max(h => h.Count) : 1;
        foreach (var h in hours)
        {
            HourlyActivity.Add(new BarItemViewModel(
                $"{h.Hour}:00", h.Count, (double)h.Count / maxHourCount * 300));
        }

        // Сводка
        Summary = analyzer.GetSummary(list);
    }
}