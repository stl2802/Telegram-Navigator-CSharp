using CommunityToolkit.Mvvm.Input;
using TelegramNavigator.Core.Cli;

namespace TelegramNavigator.Desktop.ViewModels;

public partial class HelpViewModel : ViewModelBase
{
    public string HotkeysHelp { get; } = HelpText.Hotkeys;
    public string CliHelp { get; } = HelpText.Cli;

    [RelayCommand]
    private void Close() { }
}