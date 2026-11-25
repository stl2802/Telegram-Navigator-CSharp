using CommunityToolkit.Mvvm.Input;

namespace TelegramNavigator.Desktop.ViewModels;

public partial class HelpViewModel : ViewModelBase
{
    public string HelpText { get; } =
        """
        Telegram Navigator — просмотрщик чатов, экспортированных из Telegram Desktop.

        Горячие клавиши:
          F1         — открыть эту справку
          Ctrl+O     — открыть файл result.json
          Ctrl+F     — перейти к строке поиска
          Ctrl+T     — переключить тему (светлая / тёмная)
          Ctrl+A     — выбрать все сообщения для экспорта
          Escape     — сбросить фильтры

        Вкладки:
          Сообщения — список сообщений с фильтрацией
          Медиа     — только сообщения с фото, видео, аудио
          Ссылки    — все ссылки из сообщений
          Аналитика — статистика по авторам и активность

        Экспорт в PDF будет добавлен на следующих этапах.
        """;

    [RelayCommand]
    private void Close() { }
}