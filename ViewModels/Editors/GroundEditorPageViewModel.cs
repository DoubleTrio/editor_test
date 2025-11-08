namespace AvaloniaTest.ViewModels;

public class GroundEditorPageViewModel : EditorPageViewModel
{
    public override string UniqueId => "GroundEditor";
    public override string Title => "Ground Editor With A Long Name hehehehehe";
    
    public GroundEditorPageViewModel(TabEvents tabEvents) : base(tabEvents)
    { }
}