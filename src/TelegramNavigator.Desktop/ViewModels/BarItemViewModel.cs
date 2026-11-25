namespace TelegramNavigator.Desktop.ViewModels;

/// <summary>
/// Элемент столбчатой диаграммы (без сторонних библиотек).
/// </summary>
/// <param name="Label">Подпись по оси X.</param>
/// <param name="Value">Абсолютное значение.</param>
/// <param name="BarWidth">Ширина столбца в пикселях.</param>
public record BarItemViewModel(string Label, int Value, double BarWidth);