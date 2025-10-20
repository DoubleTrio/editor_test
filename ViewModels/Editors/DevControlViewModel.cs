namespace AvaloniaTest.ViewModels;

public class DevControlViewModel : EditorPageViewModel
{
    public override string UniqueId => "DevControl";
    public override string? Title => "Dev Control";

    public TestComboBoxViewModel Fruits { get; }
    
    public DevControlViewModel(TabEvents tabEvents) : base(tabEvents)
    {
       
        Fruits = new TestComboBoxViewModel();
    }
    
    public DevControlViewModel() : base(new TabEvents())
    {
        Title = "Dev Control";
        Fruits = new TestComboBoxViewModel();
    }
    
}