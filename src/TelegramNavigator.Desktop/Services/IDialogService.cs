namespace TelegramNavigator.Desktop.Services;

/// <summary>
/// Сервис диалогов с пользователем.
/// Реализация живёт в Desktop-слое и использует Window, но ViewModel
/// знает только этот интерфейс.
/// </summary>
public interface IDialogService
{
    /// <summary>Выбрать JSON-файл чата.</summary>
    Task<string?> PickChatFileAsync();

    /// <summary>Выбрать путь для сохранения PDF.</summary>
    Task<string?> PickPdfSavePathAsync(string suggestedName);

    /// <summary>Показать окно справки по F1.</summary>
    Task ShowHelpAsync();

    /// <summary>Показать модальное окно с ошибкой.</summary>
    Task ShowErrorAsync(string title, string message);
}