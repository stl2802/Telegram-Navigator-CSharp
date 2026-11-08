using System.Globalization;
using System.Text.Json;
using TelegramNavigator.Core.Models;
using TelegramNavigator.Core.Parsing.Dto;

namespace TelegramNavigator.Core.Parsing;

/// <summary>
/// Преобразует DTO-объекты из JSON Telegram в доменные модели.
/// </summary>
public static class TelegramMessageMapper
{
    /// <summary>
    /// Преобразует одно DTO-сообщение в доменную модель <see cref="ChatMessage"/>.
    /// </summary>
    public static ChatMessage? Map(TelegramMessageDto dto, string? baseDirectory)
    {
        var date = ParseDate(dto.Date, dto.DateUnixTime);
        if (date is null)
            return null;

        var text = ExtractText(dto);
        var links = ExtractLinks(dto);
        var author = dto.From ?? dto.Actor ?? "Неизвестный";

        if (dto.Type == "service")
        {
            return new ServiceMessage(
                dto.Id, date.Value, author, text, links,
                dto.ReplyToMessageId, dto.Action ?? "unknown");
        }

        if (dto.Photo is not null)
        {
            var (filePath, isFileAvailable) = ResolveFile(dto.Photo, baseDirectory);
            return new MediaMessage(
                dto.Id, date.Value, author, text, links,
                dto.ReplyToMessageId, MediaKind.Photo, filePath, isFileAvailable);
        }

        if (dto.File is not null)
        {
            var kind = MapMediaKind(dto.MediaType);
            var (filePath, isFileAvailable) = ResolveFile(dto.File, baseDirectory);
            return new MediaMessage(
                dto.Id, date.Value, author, text, links,
                dto.ReplyToMessageId, kind, filePath, isFileAvailable);
        }

        return new TextMessage(
            dto.Id, date.Value, author, text, links, dto.ReplyToMessageId);
    }

    /// <summary>
    /// Извлекает текст из text_entities или поля "text".
    /// </summary>
    internal static string ExtractText(TelegramMessageDto dto)
    {
        if (dto.TextEntities is { Count: > 0 })
        {
            return string.Concat(dto.TextEntities.Select(e => e.Text));
        }

        return ExtractTextFromElement(dto.Text);
    }

    /// <summary>
    /// Извлекает ссылки из text_entities.
    /// </summary>
    internal static List<LinkInfo> ExtractLinks(TelegramMessageDto dto)
    {
        if (dto.TextEntities is null || dto.TextEntities.Count == 0)
            return new List<LinkInfo>();

        return dto.TextEntities
            .Where(e => e.Type == "link" || e.Type == "text_link")
            .Select(e => e.Type == "link"
                ? new LinkInfo(e.Text, null)
                : new LinkInfo(e.Href ?? e.Text, e.Text))
            .ToList();
    }

    /// <summary>
    /// Парсит дату: сначала ISO-строка "date", затем unix-время "date_unixtime".
    /// </summary>
    internal static DateTime? ParseDate(string dateStr, string unixTimeStr)
    {
        if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date))
            return date;

        if (long.TryParse(unixTimeStr, out var unixSeconds))
            return DateTimeOffset.FromUnixTimeSeconds(unixSeconds).DateTime;

        return null;
    }

    /// <summary>
    /// Определяет MediaKind по строке media_type.
    /// </summary>
    internal static MediaKind MapMediaKind(string? mediaType)
    {
        return mediaType switch
        {
            "video_file" => MediaKind.Video,
            "voice_message" => MediaKind.VoiceMessage,
            "video_message" => MediaKind.VideoMessage,
            "audio_file" => MediaKind.Audio,
            "sticker" => MediaKind.Sticker,
            "animation" => MediaKind.Animation,
            "photo" => MediaKind.Photo,
            null or "" => MediaKind.Document,
            _ => MediaKind.Document,
        };
    }

    /// <summary>
    /// Разрешает относительный путь к файлу медиа.
    /// </summary>
    internal static (string? FilePath, bool IsFileAvailable) ResolveFile(
        string? relativePath, string? baseDirectory)
    {
        if (string.IsNullOrEmpty(relativePath))
            return (null, false);

        if (relativePath.StartsWith("(File not included",
                StringComparison.OrdinalIgnoreCase))
        {
            return (relativePath, false);
        }

        if (baseDirectory is null)
            return (relativePath, false);

        var fullPath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));
        return (fullPath, File.Exists(fullPath));
    }

    private static string ExtractTextFromElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString() ?? string.Empty;

            case JsonValueKind.Array:
            {
                var sb = new System.Text.StringBuilder();
                foreach (var item in element.EnumerateArray())
                {
                    switch (item.ValueKind)
                    {
                        case JsonValueKind.String:
                            sb.Append(item.GetString());
                            break;
                        case JsonValueKind.Object:
                            if (item.TryGetProperty("text", out var textProp) &&
                                textProp.ValueKind == JsonValueKind.String)
                            {
                                sb.Append(textProp.GetString());
                            }
                            break;
                    }
                }
                return sb.ToString();
            }

            default:
                return string.Empty;
        }
    }
}