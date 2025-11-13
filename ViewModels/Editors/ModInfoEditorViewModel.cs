namespace AvaloniaTest.ViewModels;

using AvaloniaTest.Services;

public class ModInfoEditorViewModel : EditorPageViewModel
{
    public override string UniqueId => "ModInfoEditor";
    public override string? Title => "Mod Info";

    public TestComboBoxViewModel Fruits { get; }
    
    public ModInfoEditorViewModel(PageFactory pageFactory, TabEvents tabEvents, IDialogService dialogService) : base(pageFactory, tabEvents, dialogService)
    {
       
        Fruits = new TestComboBoxViewModel();
    }
    
    public ModInfoEditorViewModel() : base(new PageFactory(new DesignServiceProvider()), new TabEvents(new PageFactory(new DesignServiceProvider())), new DialogService())
    {
        Title = "Dev Control";
        Fruits = new TestComboBoxViewModel();
    }
    
}