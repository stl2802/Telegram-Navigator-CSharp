using System.Text.Json;
using System.Text.Json.Serialization;

namespace TelegramNavigator.Core.Parsing.Dto;

/// <summary>
/// Корневой объект JSON-экспорта Telegram Desktop (result.json).
/// </summary>
public class TelegramExportDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("messages")]
    public List<TelegramMessageDto> Messages { get; set; } = new();
}

/// <summary>
/// Одно сообщение из JSON-экспорта.
/// Поле "text" объявлено как JsonElement — разбор будет в маппере.
/// </summary>
public class TelegramMessageDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("date_unixtime")]
    public string DateUnixTime { get; set; } = string.Empty;

    [JsonPropertyName("from")]
    public string? From { get; set; }

    [JsonPropertyName("from_id")]
    public string? FromId { get; set; }

    [JsonPropertyName("actor")]
    public string? Actor { get; set; }

    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("text")]
    public JsonElement Text { get; set; }

    [JsonPropertyName("text_entities")]
    public List<TextEntityDto>? TextEntities { get; set; }

    [JsonPropertyName("reply_to_message_id")]
    public long? ReplyToMessageId { get; set; }

    [JsonPropertyName("forwarded_from")]
    public string? ForwardedFrom { get; set; }

    [JsonPropertyName("edited")]
    public string? Edited { get; set; }

    [JsonPropertyName("photo")]
    public string? Photo { get; set; }

    [JsonPropertyName("file")]
    public string? File { get; set; }

    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }
}

/// <summary>
/// Элемент из массива text_entities.
/// </summary>
public class TextEntityDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("href")]
    public string? Href { get; set; }
}