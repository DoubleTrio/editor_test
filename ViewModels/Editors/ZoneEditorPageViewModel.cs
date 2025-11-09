using AvaloniaTest.Services;

namespace AvaloniaTest.ViewModels;

public class ZoneEditorPageViewModel : EditorPageViewModel
{
    public override string UniqueId => "ZoneEditor";
    public override string Title => "Zone Editor";
    
    public ZoneEditorPageViewModel(TabEvents tabEvents, IDialogService dialogService) : base(tabEvents, dialogService)
    {
        
    }
    
}
    