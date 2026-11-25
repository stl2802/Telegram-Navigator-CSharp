using System.Text.Json;

namespace TelegramNavigator.Core.Settings;

/// <summary>
/// Сериализация/десериализация настроек в JSON-файл.
///
/// Методы синхронные — это осознанный выбор:
/// файл настроек маленький (менее 1 КБ), читается при старте приложения
/// и пишется при закрытии. Асинхронность в этом сценарии добавляет
/// сложность без ощутимой выгоды. Кроме того, синхронная запись
/// гарантирует, что файл будет сохранён до завершения процесса.
/// </summary>
public class JsonSettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private readonly string _filePath;

    /// <param name="filePath">
    /// Путь к файлу настроек. Если null, используется путь по умолчанию:
    /// <see cref="Environment.SpecialFolder.ApplicationData"/>/TelegramNavigator/settings.json.
    /// </param>
    public JsonSettingsService(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TelegramNavigator",
            "settings.json");
    }

    /// <inheritdoc />
    public string SettingsFilePath => _filePath;

    /// <inheritdoc />
    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(_filePath))
                return AppSettings.Default;

            var json = File.ReadAllText(_filePath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);

            return settings ?? AppSettings.Default;
        }
        catch (JsonException)
        {
            // Битый JSON — возвращаем настройки по умолчанию.
            // Приложение не должно падать из-за повреждённого файла настроек.
            return AppSettings.Default;
        }
        catch (Exception)
        {
            // Любая другая ошибка чтения (нет прав, битый диск и т.п.)
            // тоже не должна ронять приложение.
            return AppSettings.Default;
        }
    }

    /// <inheritdoc />
    public void Save(AppSettings settings)
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(_filePath, json);
    }
}