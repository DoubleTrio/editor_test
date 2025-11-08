namespace AvaloniaTest.ViewModels;

public class GroundEditorPageViewModel : EditorPageViewModel
{
    public override string UniqueId => "GroundEditor";
    public override string Title => "Ground Editor Long Name";
    
    public GroundEditorPageViewModel(TabEvents tabEvents) : base(tabEvents)
    { }
}