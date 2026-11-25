using System.Collections.ObjectModel;
using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Desktop.ViewModels;

/// <summary>
/// ViewModel для графа связей (кто кому отвечает).
/// Содержит узлы (участники) и рёбра (связи) с позициями для отрисовки на Canvas.
/// </summary>
public partial class ReplyGraphViewModel : ViewModelBase
{
    /// <summary>Узлы графа (участники) с позициями.</summary>
    public ObservableCollection<GraphNodeViewModel> Nodes { get; } = new();

    /// <summary>Рёбра графа со стартовыми/конечными координатами.</summary>
    public ObservableCollection<GraphLinkViewModel> Links { get; } = new();

    /// <summary>Ширина области отрисовки.</summary>
    public double Width { get; set; } = 600;

    /// <summary>Высота области отрисовки.</summary>
    public double Height { get; set; } = 400;

    /// <summary>Максимальное количество ответов (для нормализации толщины линий).</summary>
    public int MaxCount { get; private set; } = 1;

    private List<string> _authors = new();

    public void Update(IReadOnlyList<ChatMessage> messages, IChatAnalyzer analyzer)
    {
        var pairs = analyzer.GetReplyPairs(messages, 20);
        if (pairs.Count == 0) return;

        _authors = pairs
            .SelectMany(p => new[] { p.From, p.To })
            .Distinct()
            .OrderBy(a => a)
            .ToList();

        MaxCount = pairs.Max(p => p.Count);

        var centerX = Width / 2;
        var centerY = Height / 2;
        var radius = Math.Min(Width, Height) / 2 - 60;

        Nodes.Clear();
        for (int i = 0; i < _authors.Count; i++)
        {
            var angle = 2 * Math.PI * i / _authors.Count - Math.PI / 2;
            var x = centerX + radius * Math.Cos(angle);
            var y = centerY + radius * Math.Sin(angle);
            Nodes.Add(new GraphNodeViewModel(
                Name: _authors[i],
                X: x,
                Y: y,
                Color: GetColor(i)));
        }

        Links.Clear();
        foreach (var pair in pairs)
        {
            var fromNode = Nodes.FirstOrDefault(n => n.Name == pair.From);
            var toNode = Nodes.FirstOrDefault(n => n.Name == pair.To);
            if (fromNode is null || toNode is null) continue;

            var weight = (double)pair.Count / MaxCount;
            Links.Add(new GraphLinkViewModel(
                X1: fromNode.X, Y1: fromNode.Y,
                X2: toNode.X, Y2: toNode.Y,
                Thickness: 1 + weight * 6,
                Count: pair.Count));
        }
    }

    private static string GetColor(int index)
    {
        var colors = new[] {
            "#FF5733", "#33FF57", "#3357FF", "#FF33F1",
            "#F1FF33", "#33FFF5", "#FF8633", "#8E33FF",
            "#FF3380", "#33FF80"
        };
        return colors[index % colors.Length];
    }
}

/// <summary>
/// Узел графа — участник с позицией на Canvas.
/// </summary>
public record GraphNodeViewModel(string Name, double X, double Y, string Color);

/// <summary>
/// Ребро графа — линия между двумя узлами.
/// </summary>
public record GraphLinkViewModel(double X1, double Y1, double X2, double Y2, double Thickness, int Count);