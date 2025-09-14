using System.Reactive;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

public class AutoCompleteBinder<T> : ViewModelBase
{
    public bool CanClear => SearchText.Length > 0;
    
    public bool HasMatchStart => Items.Any(s => s.ToString().StartsWith(SearchText, StringComparison.InvariantCultureIgnoreCase));

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            this.RaisePropertyChanged(nameof(CanClear));
            this.RaisePropertyChanged(nameof(HasMatchStart));
        }
    }

    private T _lastValidItem;

    
    private T _chosenItem;

    public T ChosenItem
    {
        get => _chosenItem;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_chosenItem, value)) return;
            this.RaiseAndSetIfChanged(ref _chosenItem, value);
        }

    }

    public void CommitSelection(T item)
    {
        if (item == null) return;

        _chosenItem = item;
        _lastValidItem = item;
        SearchText = item.ToString();
        _updateModel?.Invoke(item);

        this.RaisePropertyChanged(nameof(ChosenItem));
        this.RaisePropertyChanged(nameof(CanClear));
        this.RaisePropertyChanged(nameof(HasMatchStart));
    }


    // The source list of selectable items
    public IList<T> Items { get; }

    // Optional: action to update the backing model
    private readonly Action<T> _updateModel;
    
    public bool suppressRevert;
    
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }

    public AutoCompleteBinder(IList<T> items, Action<T> updateModel = null)
    {
        Items = items;
        _updateModel = updateModel;

        ClearCommand = ReactiveCommand.Create(() =>
        {
            _lastValidItem = default;
            SearchText = string.Empty;
            ChosenItem = default;
        });
    }
    
    public void RevertIfInvalid()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            ChosenItem = default;
            return;
        }
        
        var match = Items.FirstOrDefault(x => x.ToString().Equals(SearchText, StringComparison.InvariantCultureIgnoreCase));

        if (match != null)
        {
            ChosenItem = match;
            _lastValidItem = match;
        }
        else
        {
            if (_lastValidItem != null)
                SearchText = _lastValidItem.ToString();
            else
            {
                SearchText = string.Empty;
                ChosenItem = default;
            }
        }
    }

}
