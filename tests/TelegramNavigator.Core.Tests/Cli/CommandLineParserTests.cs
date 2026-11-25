using TelegramNavigator.Core.Cli;

namespace TelegramNavigator.Core.Tests.Cli;

/// <summary>
/// Тесты для <see cref="CommandLineParser"/>.
/// </summary>
public class CommandLineParserTests
{
    [Fact]
    public void Parse_EmptyArgs_ShouldReturnDefaults()
    {
        var result = CommandLineParser.Parse([]);
        Assert.Null(result.Options.Path);
        Assert.Null(result.Options.ExportPdf);
        Assert.False(result.Options.ShowHelp);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Parse_HelpFlags_ShouldSetShowHelp()
    {
        Assert.True(CommandLineParser.Parse(["--help"]).Options.ShowHelp);
        Assert.True(CommandLineParser.Parse(["-h"]).Options.ShowHelp);
        Assert.True(CommandLineParser.Parse(["/?"]).Options.ShowHelp);
    }

    [Theory]
    [InlineData("--path", "result.json")]
    [InlineData("--author", "Иван")]
    [InlineData("--search", "hello")]
    [InlineData("--export-pdf", "out.pdf")]
    [InlineData("--top", "10")]
    public void Parse_FlagWithValue_ShouldSetOption(string flag, string value)
    {
        var result = CommandLineParser.Parse([flag, value]);
        Assert.Empty(result.Errors);

        var opt = result.Options;
        switch (flag)
        {
            case "--path":
                Assert.Equal(value, opt.Path);
                break;
            case "--author":
                Assert.Equal(value, opt.Author);
                break;
            case "--search":
                Assert.Equal(value, opt.Search);
                break;
            case "--export-pdf":
                Assert.Equal(value, opt.ExportPdf);
                break;
            case "--top":
                Assert.Equal(10, opt.Top);
                break;
        }
    }

    [Fact]
    public void Parse_From_ShouldParseDateOnly()
    {
        var result = CommandLineParser.Parse(["--from", "2024-01-15"]);
        Assert.Empty(result.Errors);
        Assert.Equal(new DateOnly(2024, 1, 15), result.Options.From);
    }

    [Fact]
    public void Parse_To_ShouldParseDateOnly()
    {
        var result = CommandLineParser.Parse(["--to", "2024-06-30"]);
        Assert.Empty(result.Errors);
        Assert.Equal(new DateOnly(2024, 6, 30), result.Options.To);
    }

    [Fact]
    public void Parse_UnknownFlag_ShouldAddError()
    {
        var result = CommandLineParser.Parse(["--unknown"]);
        Assert.Single(result.Errors);
        Assert.Contains("Неизвестный", result.Errors[0]);
    }

    [Fact]
    public void Parse_FlagWithoutValue_ShouldAddError()
    {
        var result = CommandLineParser.Parse(["--path"]);
        Assert.Single(result.Errors);
        Assert.Contains("ожидается", result.Errors[0]);
    }

    [Fact]
    public void Parse_InvalidDate_ShouldAddError()
    {
        var result = CommandLineParser.Parse(["--from", "not-a-date"]);
        Assert.Single(result.Errors);
        Assert.Contains("неверный формат", result.Errors[0]);
    }

    [Fact]
    public void Parse_InvalidTop_ShouldAddError()
    {
        var result = CommandLineParser.Parse(["--top", "abc"]);
        Assert.Single(result.Errors);
        Assert.Contains("неверное значение", result.Errors[0]);
    }

    [Fact]
    public void Parse_NegativeTop_ShouldAddError()
    {
        // "-1" начинается с '-', поэтому парсер считает, что значение не передано
        var result = CommandLineParser.Parse(["--top", "-1"]);
        Assert.NotEmpty(result.Errors);
        Assert.Null(result.Options.Top);
    }

    [Fact]
    public void ToFilter_ShouldReturnNull_WhenNoFilterOptions()
    {
        var options = new CommandLineOptions("path.json", null, null, null, null, null, null, false);
        Assert.Null(options.ToFilter());
    }

    [Fact]
    public void ToFilter_ShouldReturnFilter_WhenAuthorSet()
    {
        var options = new CommandLineOptions("path.json", null, "Alice", null, null, null, null, false);
        var filter = options.ToFilter();
        Assert.NotNull(filter);
        Assert.Equal("Alice", filter!.Author);
    }

    [Fact]
    public void Parse_AllOptions_ShouldSetCorrectly()
    {
        var result = CommandLineParser.Parse([
            "--path", "chat.json",
            "--export-pdf", "out.pdf",
            "--author", "Alice",
            "--search", "hello",
            "--from", "2024-01-01",
            "--to", "2024-06-30",
            "--top", "3"
        ]);

        Assert.Empty(result.Errors);
        var opt = result.Options;
        Assert.Equal("chat.json", opt.Path);
        Assert.Equal("out.pdf", opt.ExportPdf);
        Assert.Equal("Alice", opt.Author);
        Assert.Equal("hello", opt.Search);
        Assert.Equal(new DateOnly(2024, 1, 1), opt.From);
        Assert.Equal(new DateOnly(2024, 6, 30), opt.To);
        Assert.Equal(3, opt.Top);
        Assert.False(opt.ShowHelp);
    }
}