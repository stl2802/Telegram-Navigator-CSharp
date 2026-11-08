using System.Text.Json;
using TelegramNavigator.Core.Models;
using TelegramNavigator.Core.Parsing.Dto;

namespace TelegramNavigator.Core.Parsing;

/// <summary>
/// Парсинг чата из JSON-файла экспорта Telegram Desktop.
/// </summary>
public class TelegramJsonParser : IChatParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Асинхронно разбирает JSON-поток в доменную модель <see cref="Chat"/>.
    /// </summary>
    public async Task<Chat> ParseAsync(Stream stream, string? baseDirectory, CancellationToken ct = default)
    {
        TelegramExportDto? dto;
        try
        {
            dto = await JsonSerializer.DeserializeAsync<TelegramExportDto>(
                stream, Options, ct);
        }
        catch (JsonException ex)
        {
            throw new ChatParseException(
                "Ошибка парсинга JSON-файла: неверный формат.", ex);
        }

        if (dto is null)
        {
            throw new ChatParseException(
                "Файл пуст или не содержит данных.");
        }

        var messages = new List<ChatMessage>(dto.Messages.Count);

        foreach (var msgDto in dto.Messages)
        {
            try
            {
                var mapped = TelegramMessageMapper.Map(msgDto, baseDirectory);
                if (mapped is not null)
                    messages.Add(mapped);
            }
            catch
            {
                // Пропускаем сообщения, которые не удалось смаппить
            }
        }

        return new Chat(dto.Name, dto.Type, dto.Id, messages.AsReadOnly());
    }

    /// <summary>
    /// Открывает файл и вызывает <see cref="ParseAsync"/>.
    /// </summary>
    public async Task<Chat> ParseFileAsync(string path, CancellationToken ct = default)
    {
        if (!File.Exists(path))
        {
            throw new ChatParseException(
                $"Файл не найден: {path}");
        }

        var baseDirectory = Path.GetDirectoryName(Path.GetFullPath(path));

        await using var stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);

        return await ParseAsync(stream, baseDirectory, ct);
    }
}