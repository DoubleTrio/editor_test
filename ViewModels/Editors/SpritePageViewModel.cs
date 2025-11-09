using System;
using System.Reactive;
using AvaloniaTest.Services;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

public class SpritePageViewModel : EditorPageViewModel
{
    public ReactiveCommand<Unit, Unit> CreateATab { get; }
    public ReactiveCommand<Unit, Unit> TestDialog { get; }
    public override string Title => "Sprite Stuff";

    
    public SpritePageViewModel(TabEvents tabEvents, IDialogService dialogService) : base(tabEvents, dialogService)
    {
        CreateATab = ReactiveCommand.Create(() => tabEvents.AddChildPage(this, new SpritePageViewModel(tabEvents, dialogService)));
        TestDialog = ReactiveCommand.CreateFromTask(async () =>
            {
                var rename = new RenameWindowViewModel();
                var result = await dialogService.ShowDialogAsync<RenameWindowViewModel, bool>(rename, "Hi");
                Console.WriteLine(rename.Name);

            }
            
        );
    }
}