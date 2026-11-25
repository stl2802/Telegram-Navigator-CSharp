using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using TelegramNavigator.Desktop.ViewModels;

namespace TelegramNavigator.Desktop.Views;

/// <summary>
/// UserControl для отрисовки графа связей на Canvas.
/// </summary>
public class ReplyGraphView : Control
{
    public static readonly StyledProperty<ReplyGraphViewModel?> GraphProperty =
        AvaloniaProperty.Register<ReplyGraphView, ReplyGraphViewModel?>(nameof(Graph));

    public ReplyGraphViewModel? Graph
    {
        get => GetValue(GraphProperty);
        set => SetValue(GraphProperty, value);
    }

    static ReplyGraphView()
    {
        AffectsRender<ReplyGraphView>(GraphProperty);
    }

    public ReplyGraphView()
    {
        ClipToBounds = true;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var graph = Graph;
        if (graph is null) return;

        // Фон
        context.FillRectangle(Brushes.White, new Rect(0, 0, Bounds.Width, Bounds.Height));

        // Рёбра
        foreach (var link in graph.Links)
        {
            var pen = new Pen(Brushes.Gray, link.Thickness, lineCap: PenLineCap.Round);
            context.DrawLine(pen, new Point(link.X1, link.Y1), new Point(link.X2, link.Y2));

            var midX = (link.X1 + link.X2) / 2;
            var midY = (link.Y1 + link.Y2) / 2;
            DrawText(context, link.Count.ToString(), midX - 10, midY - 10, 12);
        }

        // Узлы
        foreach (var node in graph.Nodes)
        {
            var brush = new SolidColorBrush(Color.Parse(node.Color));
            const double radius = 18.0;

            context.DrawEllipse(brush, new Pen(Brushes.Black, 2), new Point(node.X, node.Y), radius, radius);
            DrawText(context, node.Name, node.X + radius + 4, node.Y - 8, 14);
        }
    }

    private static void DrawText(DrawingContext context, string text, double x, double y, double fontSize)
    {
        var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            Typeface.Default, fontSize, Brushes.Black);
        context.DrawText(ft, new Point(x, y));
    }
}