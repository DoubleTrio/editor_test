namespace AvaloniaTest.ViewModels;

public class GroundEditorPageViewModel : EditorPageViewModel
{
    public override string UniqueId => "GroundEditor";
    public override string Title => "Ground Editor";
    
    public GroundEditorPageViewModel(TabEvents tabEvents) : base(tabEvents)
    { }
}