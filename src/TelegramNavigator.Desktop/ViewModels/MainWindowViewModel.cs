using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Models;
using TelegramNavigator.Core.Parsing;
using TelegramNavigator.Core.Services;
using TelegramNavigator.Desktop.Services;

namespace TelegramNavigator.Desktop.ViewModels;

/// <summary>
/// Главная ViewModel приложения. Связывает Core с UI через привязки и команды.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IChatParser _parser;
    private readonly IChatAnalyzer _analyzer;
    private readonly ChatSession _session;
    private readonly IDialogService _dialogs;

    public MainWindowViewModel(
        IChatParser parser,
        IChatAnalyzer analyzer,
        ChatSession session,
        IDialogService dialogs)
    {
        _parser = parser;
        _analyzer = analyzer;
        _session = session;
        _dialogs = dialogs;

        _session.ChatLoaded += OnChatLoaded;
        _session.FilterChanged += OnFilterChanged;

        Authors.Add("Все");
    }

    // ===== Коллекции =====

    public ObservableCollection<MessageItemViewModel> Messages { get; } = new();
    public ObservableCollection<MessageItemViewModel> MediaItems { get; } = new();
    public ObservableCollection<LinkEntryViewModel> LinkItems { get; } = new();
    public ObservableCollection<string> Authors { get; } = new();

    // ===== Свойства фильтров =====

    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private string? _selectedAuthor;
    [ObservableProperty] private DateTimeOffset? _dateFrom;
    [ObservableProperty] private DateTimeOffset? _dateTo;
    [ObservableProperty] private bool _onlyWithMedia;
    [ObservableProperty] private bool _onlyWithLinks;

    partial void OnSearchTextChanged(string? value) => ApplyCurrentFilter();
    partial void OnSelectedAuthorChanged(string? value) => ApplyCurrentFilter();
    partial void OnDateFromChanged(DateTimeOffset? value) => ApplyCurrentFilter();
    partial void OnDateToChanged(DateTimeOffset? value) => ApplyCurrentFilter();
    partial void OnOnlyWithMediaChanged(bool value) => ApplyCurrentFilter();
    partial void OnOnlyWithLinksChanged(bool value) => ApplyCurrentFilter();

    // ===== Состояние =====

    [ObservableProperty] private MessageItemViewModel? _selectedMessage;
    [ObservableProperty] private string _statusText = "Откройте файл result.json (Ctrl+O)";
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isDarkTheme;

    /// <summary>
    /// Аналитика — лениво создаётся при первом обращении.
    /// </summary>
    public AnalyticsViewModel Analytics { get; } = new();

    // ===== Команды =====

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var path = await _dialogs.PickChatFileAsync();
        if (path is null) return;
        await LoadFileAsync(path);
    }

    /// <summary>
    /// Загружает чат из файла. Можно вызвать из code-behind или CLI.
    /// </summary>
    public async Task LoadFileAsync(string path)
    {
        IsBusy = true;
        StatusText = "Загрузка...";

        try
        {
            var chat = await _parser.ParseFileAsync(path);
            _session.Load(chat);
            StatusText = $"Загружен чат «{chat.Name}» — {chat.Messages.Count} сообщ.";
        }
        catch (ChatParseException ex)
        {
            await _dialogs.ShowErrorAsync("Ошибка загрузки", ex.Message);
            StatusText = "Ошибка загрузки";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = null;
        SelectedAuthor = null;
        DateFrom = null;
        DateTo = null;
        OnlyWithMedia = false;
        OnlyWithLinks = false;
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        Application.Current!.RequestedThemeVariant =
            IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
    }

    [RelayCommand]
    private async Task ShowHelpAsync()
    {
        await _dialogs.ShowHelpAsync();
    }

    [RelayCommand]
    private void SelectAllForExport()
    {
        foreach (var item in Messages)
            item.IsSelectedForExport = true;
    }

    // ===== Обработчики событий =====

    private void OnChatLoaded(object? sender, ChatLoadedEventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => UpdateCollections(e.Chat));
    }

    private void OnFilterChanged(object? sender, EventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            Messages.Clear();
            foreach (var msg in _session.FilteredMessages)
                Messages.Add(new MessageItemViewModel(msg));
        });
    }

    private void UpdateCollections(Chat chat)
    {
        Messages.Clear();
        foreach (var msg in _session.FilteredMessages)
            Messages.Add(new MessageItemViewModel(msg));

        MediaItems.Clear();
        foreach (var msg in _session.Media)
            MediaItems.Add(new MessageItemViewModel(msg));

        LinkItems.Clear();
        foreach (var entry in _session.Links)
            LinkItems.Add(new LinkEntryViewModel(entry));

        Authors.Clear();
        Authors.Add("Все");
        foreach (var author in chat.Messages
                     .Select(m => m.Author)
                     .Where(a => !string.IsNullOrEmpty(a))
                     .Distinct()
                     .OrderBy(a => a))
        {
            Authors.Add(author);
        }

        Analytics.Update(chat.Messages, _analyzer);
    }

    private void ApplyCurrentFilter()
    {
        var filter = new MessageFilter(
            string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
            SelectedAuthor is null or "Все" ? null : SelectedAuthor,
            DateFrom.HasValue ? DateOnly.FromDateTime(DateFrom.Value.Date) : null,
            DateTo.HasValue ? DateOnly.FromDateTime(DateTo.Value.Date) : null,
            OnlyWithMedia,
            OnlyWithLinks);

        _session.ApplyFilter(filter);
    }
}

/// <summary>
/// ViewModel для отображения ссылки.
/// </summary>
public partial class LinkEntryViewModel : ViewModelBase
{
    public LinkEntryViewModel(LinkEntry entry)
    {
        MessageId = entry.Message.Id;
        Author = entry.Message.Author;
        Url = entry.Link.Url;
        Title = entry.Link.Title ?? Url;
        DateText = entry.Message.Date.ToString("d MMM yyyy, HH:mm");
    }

    public long MessageId { get; }
    public string Author { get; }
    public string Url { get; }
    public string Title { get; }
    public string DateText { get; }
}