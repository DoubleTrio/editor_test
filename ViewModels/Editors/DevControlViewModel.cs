using AvaloniaTest.Services;

namespace AvaloniaTest.ViewModels;

public class DevControlViewModel : EditorPageViewModel
{
    public override string UniqueId => "DevControl";
    public override string? Title => "Dev Control";

    public TestComboBoxViewModel Fruits { get; }
    
    public DevControlViewModel(TabEvents tabEvents, IDialogService dialogService) : base(tabEvents, dialogService)
    {
       
        Fruits = new TestComboBoxViewModel();
    }
    
    public DevControlViewModel() : base(new TabEvents(), new DialogService())
    {
        Title = "Dev Control";
        Fruits = new TestComboBoxViewModel();
    }
    
}