using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegramNavigator.Core.Analytics;
using TelegramNavigator.Core.Export;
using TelegramNavigator.Core.Models;
using TelegramNavigator.Core.Parsing;
using TelegramNavigator.Core.Services;
using TelegramNavigator.Core.Settings;
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
    private readonly IChatExporter _exporter;
    private readonly ISettingsService _settings;

    public MainWindowViewModel(
        IChatParser parser,
        IChatAnalyzer analyzer,
        ChatSession session,
        IDialogService dialogs,
        IChatExporter exporter,
        ISettingsService settings)
    {
        _parser = parser;
        _analyzer = analyzer;
        _session = session;
        _dialogs = dialogs;
        _exporter = exporter;
        _settings = settings;

        _session.ChatLoaded += OnChatLoaded;
        _session.FilterChanged += OnFilterChanged;

        Authors.Add("Все");
    }

    /// <summary>
    /// Событие: запрос фокуса на поле поиска (Ctrl+F).
    /// ViewModel не должна знать про UI, поэтому code-behind подписывается и ставит фокус.
    /// </summary>
    public event Action? FocusSearchRequested;

    [RelayCommand]
    private void FocusSearch()
    {
        FocusSearchRequested?.Invoke();
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
    /// Аналитика.
    /// </summary>
    public AnalyticsViewModel Analytics { get; } = new();

    /// <summary>
    /// Граф связей (кто кому отвечает).
    /// </summary>
    public ReplyGraphViewModel ReplyGraph { get; } = new();

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

    [RelayCommand]
    private async Task ExportPdfAsync()
    {
        if (_session.CurrentChat is null)
        {
            await _dialogs.ShowErrorAsync("Экспорт PDF", "Сначала откройте чат.");
            return;
        }

        // Определяем, какие сообщения экспортировать
        var selected = Messages.Where(m => m.IsSelectedForExport).Select(m => m.Message).ToList();
        var toExport = selected.Count > 0 ? selected : _session.FilteredMessages.ToList();

        if (toExport.Count == 0)
        {
            await _dialogs.ShowErrorAsync("Экспорт PDF", "Нет сообщений для экспорта.");
            return;
        }

        var suggestedName = $"{SanitizeFileName(_session.CurrentChat.Name)}.pdf";
        var path = await _dialogs.PickPdfSavePathAsync(suggestedName);
        if (path is null) return;

        IsBusy = true;
        StatusText = "Экспорт в PDF...";

        try
        {
            await _exporter.ExportAsync(_session.CurrentChat.Name, toExport, path);
            StatusText = $"PDF сохранён: {path}";
        }
        catch (Exception ex)
        {
            await _dialogs.ShowErrorAsync("Ошибка экспорта", ex.Message);
            StatusText = "Ошибка экспорта PDF";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Удаляет из имени файла недопустимые символы.
    /// </summary>
    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(name.Select(ch => invalid.Contains(ch) ? '_' : ch));
    }

    // ===== Настройки =====

    /// <summary>
    /// Создаёт объект настроек из текущего состояния ViewModel.
    /// </summary>
    public AppSettings CreateSettings()
    {
        var recentFiles = new List<string>();

        // Сохраняем текущий LastPath первым
        if (_session.CurrentChat is not null && !string.IsNullOrEmpty(LastPath))
        {
            recentFiles.Add(LastPath);
            // Добавляем остальные недавние файлы
            foreach (var f in RecentFiles)
            {
                if (f != LastPath)
                    recentFiles.Add(f);
            }
        }
        else
        {
            recentFiles.AddRange(RecentFiles);
        }

        return new AppSettings(
            Theme: IsDarkTheme ? "Dark" : "Light",
            LastPath: LastPath,
            SearchText: SearchText,
            Author: SelectedAuthor is "Все" ? null : SelectedAuthor,
            OnlyWithMedia: OnlyWithMedia,
            OnlyWithLinks: OnlyWithLinks,
            RecentFiles: recentFiles.Take(5).ToList().AsReadOnly());
    }

    /// <summary>
    /// Применяет загруженные настройки к ViewModel.
    /// </summary>
    public void ApplySettings(AppSettings settings)
    {
        // Тема
        IsDarkTheme = settings.Theme == "Dark";
        Application.Current!.RequestedThemeVariant =
            IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;

        // Фильтры
        SearchText = settings.SearchText;
        SelectedAuthor = settings.Author;
        OnlyWithMedia = settings.OnlyWithMedia;
        OnlyWithLinks = settings.OnlyWithLinks;

        // Недавние файлы
        RecentFiles.Clear();
        foreach (var f in settings.RecentFiles)
            RecentFiles.Add(f);

        LastPath = settings.LastPath;
    }

    // ===== Recent files =====

    /// <summary>
    /// Текущий LastPath для настройки.
    /// </summary>
    private string? LastPath { get; set; }

    /// <summary>
    /// Список недавно открытых файлов.
    /// </summary>
    public ObservableCollection<string> RecentFiles { get; } = new();

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
        ReplyGraph.Update(chat.Messages, _analyzer);
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