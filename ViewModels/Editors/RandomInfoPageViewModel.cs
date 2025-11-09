using AvaloniaTest.Services;

namespace AvaloniaTest.ViewModels;

public class RandomInfoPageViewModel : EditorPageViewModel
{
    public override bool AddNewTab => false;
    
    public override string Title => "Random Page Info";
    public RandomInfoPageViewModel(TabEvents tabEvents, IDialogService dialogService) : base(tabEvents, dialogService)
    {
    }
    
}