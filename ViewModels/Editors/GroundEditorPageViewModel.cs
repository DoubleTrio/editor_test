namespace AvaloniaTest.ViewModels;

public class GroundEditorPageViewModel : EditorPageViewModel
{
    public override string? UniqueId => "GroundEditor";
    
    public GroundEditorPageViewModel(TabEvents tabEvents) : base(tabEvents)
    {
        Title = "Ground Editor";
    }
}