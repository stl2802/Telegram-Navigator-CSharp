using CommunityToolkit.Mvvm.ComponentModel;
using TelegramNavigator.Core.Models;

namespace TelegramNavigator.Desktop.ViewModels;

/// <summary>
/// Обёртка над ChatMessage для отображения в списке.
/// </summary>
public partial class MessageItemViewModel : ViewModelBase
{
    private readonly ChatMessage _message;

    public MessageItemViewModel(ChatMessage message)
    {
        _message = message;
    }

    public ChatMessage Message => _message;
    public string Author => _message.Author;
    public string Text => _message.Text;
    public bool IsService => _message is ServiceMessage;
    public bool HasMedia => _message.HasMedia;
    public int LinksCount => _message.Links.Count;
    public long Id => _message.Id;

    /// <summary>
    /// Форматированная дата: "1 июн 2024, 10:30"
    /// </summary>
    public string DateText => _message.Date.ToString("d MMM yyyy, HH:mm");

    /// <summary>
    /// Эмодзи-иконка по типу сообщения.
    /// </summary>
    public string Icon => _message switch
    {
        MediaMessage m => m.Kind switch
        {
            MediaKind.Photo => "📷",
            MediaKind.Video => "🎬",
            MediaKind.VideoMessage => "🔄",
            MediaKind.VoiceMessage => "🎙️",
            MediaKind.Audio => "🎵",
            MediaKind.Sticker => "🎨",
            MediaKind.Animation => "🎞️",
            MediaKind.Document => "📄",
            _ => "📎",
        },
        ServiceMessage => "🔔",
        _ => "💬",
    };

    [ObservableProperty]
    private bool _isSelectedForExport;
}