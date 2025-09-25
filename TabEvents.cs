using System;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest;

public class TabEvents
{
    public event Action<EditorPageViewModel, EditorPageViewModel> AddChildTabEvent;
    public event Action<EditorPageViewModel> AddTopLevelTabEvent;
    
    public event Action<EditorPageViewModel> AddTemporaryTabEvent;
    public event Action<EditorPageViewModel> RemoveTabEvent;

    public void AddChildPage<T>(EditorPageViewModel parent) where T : EditorPageViewModel, new()
    {
        var child = new T();
        AddChildTabEvent.Invoke(parent, child);
    }
    
    public void AddChildPage(EditorPageViewModel parent, EditorPageViewModel child)
    {
        AddChildTabEvent.Invoke(parent, child);
    }

    public void AddTopLevelTab<T>() where T : EditorPageViewModel, new()
    {
        var tab = new T();
        AddTopLevelTabEvent.Invoke(tab);
    }
    
    public void AddTopLevelTab(EditorPageViewModel tab)
    {
        AddTopLevelTabEvent.Invoke(tab);
    }


    public void AddTemporaryTab<T>() where T : EditorPageViewModel, new()
    {
        var tab = new T();
        AddTemporaryTabEvent.Invoke(tab);
    }

    public void RemoveTab(EditorPageViewModel page)
    {
        RemoveTabEvent.Invoke(page);
    }
}