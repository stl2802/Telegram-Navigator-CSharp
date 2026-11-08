namespace TelegramNavigator.Core.Models;

/// <summary>
/// Информация о ссылке, найденной в тексте сообщения.
/// </summary>
/// <param name="Url">URL ссылки.</param>
/// <param name="Title">Название ссылки (может совпадать с URL или быть null).</param>
public record LinkInfo(string Url, string? Title);