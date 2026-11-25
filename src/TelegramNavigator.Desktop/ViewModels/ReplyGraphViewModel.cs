using System.Collections.ObjectModel;
using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Desktop.ViewModels;

/// <summary>
/// ViewModel для вкладки графа связей (кто кому отвечает).
/// </summary>
public partial class ReplyGraphViewModel : ViewModelBase
{
    public ObservableCollection<ReplyEdgeViewModel> Edges { get; } = new();

    public void Update(IReadOnlyList<ChatMessage> messages, IChatAnalyzer analyzer)
    {
        Edges.Clear();
        var pairs = analyzer.GetReplyPairs(messages, 20);
        foreach (var p in pairs)
            Edges.Add(new ReplyEdgeViewModel(p));
    }
}

/// <summary>
/// ViewModel для одного ребра графа ответов.
/// </summary>
public partial class ReplyEdgeViewModel : ViewModelBase
{
    public ReplyEdgeViewModel(ReplyEdge edge)
    {
        From = edge.From;
        To = edge.To;
        Count = edge.Count;
    }

    public string From { get; }
    public string To { get; }
    public int Count { get; }
}