using System.Reactive;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

public class SpritePageViewModel : EditorPageViewModel
{
    public ReactiveCommand<Unit, Unit> CreateATab { get; }
    public override string Title => "Sprite Stuff";

    
    public SpritePageViewModel(TabEvents tabEvents) : base(tabEvents)
    {
        CreateATab = ReactiveCommand.Create(() => tabEvents.AddChildPage(this, new SpritePageViewModel(tabEvents)));
    }
}