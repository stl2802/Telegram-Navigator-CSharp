using TelegramNavigator.Core.Export;
using TelegramNavigator.Core.Tests.Helpers;

namespace TelegramNavigator.Core.Tests.Export;

/// <summary>
/// Тесты для <see cref="PdfChatExporter"/>.
/// </summary>
public class PdfChatExporterTests
{
    private readonly PdfChatExporter _exporter = new();

    [Fact]
    public async Task ExportAsync_ThreeMessages_ShouldCreateNonEmptyPdf()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"tn_test_{Guid.NewGuid()}.pdf");
        try
        {
            var messages = TestMessages.ThreeMessages();
            await _exporter.ExportAsync("Test Chat", messages, outputPath);

            Assert.True(File.Exists(outputPath));
            var content = await File.ReadAllBytesAsync(outputPath);
            Assert.NotEmpty(content);
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task ExportAsync_ShouldStartWithPdfHeader()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"tn_test_{Guid.NewGuid()}.pdf");
        try
        {
            var messages = TestMessages.ThreeMessages();
            await _exporter.ExportAsync("Test Chat", messages, outputPath);

            var content = await File.ReadAllBytesAsync(outputPath);
            // PDF-файл начинается с "%PDF"
            Assert.Equal(0x25, content[0]); // '%'
            Assert.Equal(0x50, content[1]); // 'P'
            Assert.Equal(0x44, content[2]); // 'D'
            Assert.Equal(0x46, content[3]); // 'F'
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task ExportAsync_EmptyMessages_ShouldStillCreatePdf()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"tn_test_{Guid.NewGuid()}.pdf");
        try
        {
            await _exporter.ExportAsync("Empty", Array.Empty<Core.Models.ChatMessage>(), outputPath);
            Assert.True(File.Exists(outputPath));
            Assert.True(new FileInfo(outputPath).Length > 0);
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task ExportAsync_ShouldCreateOutputDirectory()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"tn_nested_{Guid.NewGuid()}");
        var outputPath = Path.Combine(dir, "sub", "output.pdf");
        try
        {
            var messages = TestMessages.ThreeMessages();
            await _exporter.ExportAsync("Test", messages, outputPath);
            Assert.True(File.Exists(outputPath));
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
            if (Directory.Exists(dir))
                Directory.Delete(dir, recursive: true);
        }
    }
}