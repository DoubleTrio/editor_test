using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using AvaloniaTest.Services;
using AvaloniaTest.Views;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _contentViewModel;
    
    
    // Example lists
    public IList<string> Roles { get; } = new List<string> { "Leader", "Member", "Support", "Observer", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" } .OrderBy(r => r)
        .ToList();
    
    // AutoComplete binders
    public AutoCompleteBinder<string> RoleBinder { get; }
    public MainWindowViewModel()
    {
        var service = new ToDoService();
        ToDoList = new ToDoListViewModel(service.GetItems());
        _contentViewModel = ToDoList;
        
        RoleBinder = new AutoCompleteBinder<string>(Roles, (_) => { });
    }
    
    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
    }

    public ToDoListViewModel ToDoList { get; }
}