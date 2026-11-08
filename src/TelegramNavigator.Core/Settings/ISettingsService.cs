namespace TelegramNavigator.Core.Settings;

/// <summary>
/// Сервис загрузки и сохранения настроек приложения.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Загружает настройки из файла. Если файл отсутствует или повреждён,
    /// возвращает <see cref="AppSettings.Default"/>.
    /// </summary>
    AppSettings Load();

    /// <summary>
    /// Сохраняет настройки в файл.
    /// </summary>
    void Save(AppSettings settings);

    /// <summary>
    /// Полный путь к файлу настроек.
    /// </summary>
    string SettingsFilePath { get; }
}