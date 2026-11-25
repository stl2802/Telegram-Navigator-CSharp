using TelegramNavigator.Core.Settings;

namespace TelegramNavigator.Core.Tests.Settings;

/// <summary>
/// Тесты для <see cref="JsonSettingsService"/>.
/// </summary>
public class JsonSettingsServiceTests : IDisposable
{
    private readonly string _tempFile;
    private readonly JsonSettingsService _service;

    public JsonSettingsServiceTests()
    {
        _tempFile = Path.Combine(Path.GetTempPath(), $"tn_settings_{Guid.NewGuid()}.json");
        _service = new JsonSettingsService(_tempFile);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

    [Fact]
    public void Load_WhenFileMissing_ShouldReturnDefault()
    {
        var settings = _service.Load();
        Assert.Equal(AppSettings.Default, settings);
    }

    [Fact]
    public void Load_WhenFileIsCorruptJson_ShouldReturnDefault()
    {
        File.WriteAllText(_tempFile, "this is not json{");
        var settings = _service.Load();
        Assert.Equal(AppSettings.Default, settings);
    }

    [Fact]
    public void Save_Then_Load_ShouldReturnSameSettings()
    {
        var original = new AppSettings(
            Theme: "Dark",
            LastPath: "/path/to/file",
            SearchText: "hello",
            Author: "Alice",
            OnlyWithMedia: true,
            OnlyWithLinks: false,
            RecentFiles: new[] { "file1.json", "file2.json" });

        _service.Save(original);

        var loaded = _service.Load();
        Assert.Equal(original.Theme, loaded.Theme);
        Assert.Equal(original.LastPath, loaded.LastPath);
        Assert.Equal(original.SearchText, loaded.SearchText);
        Assert.Equal(original.Author, loaded.Author);
        Assert.Equal(original.OnlyWithMedia, loaded.OnlyWithMedia);
        Assert.Equal(original.OnlyWithLinks, loaded.OnlyWithLinks);
        Assert.Equal(original.RecentFiles.OrderBy(x => x), loaded.RecentFiles.OrderBy(x => x));
    }

    [Fact]
    public void Save_ShouldCreateDirectoryIfMissing()
    {
        var deepPath = Path.Combine(Path.GetTempPath(), $"tn_deep_{Guid.NewGuid()}", "sub", "settings.json");
        try
        {
            var service = new JsonSettingsService(deepPath);
            service.Save(AppSettings.Default);
            Assert.True(File.Exists(deepPath));
        }
        finally
        {
            if (File.Exists(deepPath))
                File.Delete(deepPath);
            var dir = Path.GetDirectoryName(deepPath)!;
            if (Directory.Exists(dir))
                Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void SettingsFilePath_ShouldReturnConfiguredPath()
    {
        var service = new JsonSettingsService(_tempFile);
        Assert.Equal(_tempFile, service.SettingsFilePath);
    }

    [Fact]
    public void DefaultConstructor_ShouldUseAppDataPath()
    {
        var service = new JsonSettingsService();
        Assert.Contains("TelegramNavigator", service.SettingsFilePath);
        Assert.EndsWith("settings.json", service.SettingsFilePath);
    }
}