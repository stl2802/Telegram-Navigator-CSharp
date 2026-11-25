using TelegramNavigator.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TelegramNavigator.Core.Export;

/// <summary>
/// Экспорт сообщений чата в PDF через QuestPDF (синхронная библиотека).
/// 
/// Установка лицензии:
///   Статический конструктор устанавливает LicenseType.Community,
///   что разрешает бесплатное некоммерческое использование.
///   Для коммерческого использования требуется лицензия.
/// </summary>
public class PdfChatExporter : IChatExporter
{
    static PdfChatExporter()
    {
        // Единоразовая настройка лицензии QuestPDF.
        // Community-версия бесплатна для некоммерческого использования
        // и накладывает водяной знак на сгенерированные PDF.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Асинхронно экспортирует список сообщений в PDF-файл.
    /// QuestPDF — синхронная библиотека, поэтому генерация выполняется
    /// в Task.Run, чтобы не блокировать UI-поток.
    /// </summary>
    public async Task ExportAsync(
        string chatName,
        IReadOnlyList<ChatMessage> messages,
        string outputPath,
        CancellationToken ct = default)
    {
        // Создаём папку вывода, если её нет
        var dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        await Task.Run(() => GeneratePdf(chatName, messages, outputPath), ct);
    }

    private static void GeneratePdf(
        string chatName,
        IReadOnlyList<ChatMessage> messages,
        string outputPath)
    {
        // Шрифт Noto Sans с кириллицей можно скачать с Google Fonts:
        //   https://fonts.google.com/noto/specimen/Noto+Sans
        // Положите файл NotoSans-Regular.ttf в Assets/Fonts/ проекта Desktop
        // и установите CopyToOutputDirectory = PreserveNewest в .csproj.
        //
        // QuestPDF 2024.12 использует SkiaSharp для рендеринга.
        // Если кириллица не отображается (квадратики), установите
        // Noto Sans в систему или добавьте ttf в Assets/Fonts.
        // QuestPDF поддерживает системные шрифты через SkiaSharp
        // без явной регистрации через FontManager.

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(10));

                // ===== Header =====
                page.Header().Column(col =>
                {
                    col.Item().Text(chatName)
                        .FontSize(20)
                        .Bold();

                    col.Item().Text(
                        $"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm} | " +
                        $"Сообщений: {messages.Count}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken2);
                });

                // ===== Content =====
                page.Content().Column(col =>
                {
                    foreach (var msg in messages)
                    {
                        col.Item().PaddingBottom(8).Column(msgCol =>
                        {
                            // Автор (жирным)
                            msgCol.Item().Text(msg.Author)
                                .FontSize(11)
                                .Bold()
                                .FontColor(Colors.Blue.Darken3);

                            // Дата (серым)
                            msgCol.Item().Text(msg.Date.ToString("dd.MM.yyyy HH:mm"))
                                .FontSize(8)
                                .FontColor(Colors.Grey.Darken1);

                            // Текст сообщения
                            if (!string.IsNullOrEmpty(msg.Text))
                            {
                                msgCol.Item().PaddingTop(2).Text(msg.Text)
                                    .FontSize(10);
                            }

                            // Медиа-вложение
                            if (msg is MediaMessage media)
                            {
                                var fileName = media.FilePath is not null
                                    ? Path.GetFileName(media.FilePath)
                                    : "файл";
                                var kindLabel = media.Kind switch
                                {
                                    MediaKind.Photo => "Фото",
                                    MediaKind.Video => "Видео",
                                    MediaKind.VideoMessage => "Кружок",
                                    MediaKind.VoiceMessage => "Голосовое",
                                    MediaKind.Audio => "Аудио",
                                    MediaKind.Sticker => "Стикер",
                                    MediaKind.Animation => "GIF",
                                    MediaKind.Document => "Документ",
                                    _ => "Медиа"
                                };
                                msgCol.Item().Text($"[{kindLabel}: {fileName}]")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken3)
                                    .Italic();
                            }

                            // Ссылки
                            if (msg.Links.Count > 0)
                            {
                                foreach (var link in msg.Links)
                                {
                                    var display = link.Title ?? link.Url;
                                    msgCol.Item().Text($"🔗 {display}")
                                        .FontSize(9)
                                        .FontColor(Colors.Blue.Medium);
                                }
                            }
                        });

                        // Разделитель между сообщениями
                        col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                    }
                });

                // ===== Footer (номера страниц) =====
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Страница ");
                    x.CurrentPageNumber();
                    x.Span(" из ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf(outputPath);
    }
}