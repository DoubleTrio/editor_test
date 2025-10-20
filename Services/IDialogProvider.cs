using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Services;

public interface IDialogProvider
{
    DialogViewModel? Dialog { get; set; }
}