namespace AvaloniaTest.ViewModels;

public class ZoneEditorPageViewModel : EditorPageViewModel
{
    public override string? UniqueId => "ZoneEditor";
    
    public ZoneEditorPageViewModel(TabEvents tabEvents) : base(tabEvents)
    {
        Title = "Zone Editor";
    }
    
}
    