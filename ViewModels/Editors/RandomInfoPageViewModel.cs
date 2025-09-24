namespace AvaloniaTest.ViewModels;

public class RandomInfoPageViewModel : EditorPageViewModel
{
    public override bool AddNewTab => false;
    
    public RandomInfoPageViewModel(TabEvents tabEvents) : base(tabEvents)
    {
        Title = "Random Page Info";
    }
    
}