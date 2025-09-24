namespace AvaloniaTest.ViewModels;

public class DevControlViewModel : EditorPageViewModel
{
    public override string? UniqueId => "DevControl";
    
    public DevControlViewModel(TabEvents tabEvents) : base(tabEvents)
    {
        Title = "Dev Control";
    }
    
}