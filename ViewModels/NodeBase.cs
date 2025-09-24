using System;
using System.Collections.Specialized;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

using System.Collections.ObjectModel;

public class NodeBase : ViewModelBase
{
    public ObservableCollection<NodeBase> SubNodes { get; set; }

    // public NodeBase? Parent { get; internal set; } 
    
    private string _title = "";

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
    public string Icon { get; }

    public NodeBase(string title = "", string? icon = null)
    {
        Title = title;
        Icon = icon ?? "";
        SubNodes = new ObservableCollection<NodeBase>();
        IsExpanded = false;
        // SubNodes.CollectionChanged += OnSubNodesChanged;
    }
    
    // public NodeBase(string title, string icon, ObservableCollection<NodeBase> subNodes)
    // {
    //     Title = title;
    //     Icon = icon;
    //     SubNodes = subNodes;
    //     IsExpanded = false;
    //     SubNodes.CollectionChanged += OnSubNodesChanged;
    // }
    
    
    private bool _isExpanded = false;
    public bool IsExpanded
    {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }
    
    private bool _isVisible = true;
    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }
    
    // private void OnSubNodesChanged(object? s, NotifyCollectionChangedEventArgs e) {
    //     if (e.NewItems != null)
    //         foreach(NodeBase child in e.NewItems) child.Parent = this;
    //     if (e.OldItems != null)
    //         foreach(NodeBase child in e.OldItems) child.Parent = null;
    // }


}

public class OpenEditorNode : NodeBase
{
    // public string EditorTabTitle { get; }

    // public ReactiveCommand<Unit, Unit> OpenEditorCommand { get; }

    public string EditorKey { get; }
    
    public OpenEditorNode(string title, string? icon = null, string editorKey = "") 
        : base(title, icon ?? "")
    {
        EditorKey = editorKey;
    }
}

public class ActionDataNode : NodeBase
{
    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public ActionDataNode(string title, string? icon = null) 
        : base(title, icon ?? "")
    {
        AddCommand = ReactiveCommand.Create(() => Console.WriteLine("Add command"));
        DeleteCommand = ReactiveCommand.Create(() => Console.WriteLine("Delete command"));
    }
}

public class DataItemNode : OpenEditorNode
{
    // public ReactiveCommand<Unit, Unit> DataDeleteCommand { get; }

    public string ItemKey { get; }
    
    private string _value = "";
    public string Value 
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }


    public DataItemNode(string itemKey, string editorKey, string? title, string? icon = null) 
        : base(title ?? "", icon ?? "")
    {
        // this.WhenAnyValue(x => x.Value).Subscribe(v => Title = $"{Key}: {v}");
        
        // DataDeleteCommand = ReactiveCommand.Create(() => Console.WriteLine("Delete command"));
    }
}


public class DataRootNode : OpenEditorNode
{
    // Change to enum later!
    private readonly string _dataType;
    private readonly string key;
    
    // TODO: Prompt the user to add the page... for that datatype
    public ReactiveCommand<DataItemNode, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> AddCommand2 { get; }
    public ReactiveCommand<DataItemNode, Unit> DeleteCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ReIndexCommand { get; }
    public ReactiveCommand<Unit, Unit> ResaveAllAsFileCommand { get; }
    public ReactiveCommand<Unit, Unit> ResaveAllAsDiffCommand { get; }
    // public ReactiveCommand<DataItemNode, Unit> PatchCommand { get; }
    // public ReactiveCommand<DataItemNode, Unit> PatchCommand { get; }
    // public ReactiveCommand<string, Unit> PatchCommand { get; }
    
    
    public ReactiveCommand<DataItemNode, Unit> ResaveAsFile { get; }
    public ReactiveCommand<DataItemNode, Unit> ResaveAsPatch { get; }
    public DataRootNode(string dataType, string editorKey, string title, string? icon = null) 
        : base(title, icon ?? "", editorKey)
    {
        _dataType = dataType;

        DeleteCommand = ReactiveCommand.Create<DataItemNode>(node =>
        {
            SubNodes.Remove(node);
            Console.WriteLine($"Deleting {node.Title} of type {_dataType}");
            // DataManager.Remove(assetName, _dataType);
        });
        
        AddCommand = ReactiveCommand.Create<DataItemNode>(node =>
        {
            SubNodes.Add(node);
            Console.WriteLine($"Now adding a {node.Title} of type {_dataType}");
            
            
            // DataManager.Remove(assetName, _dataType);
        });

        ReIndexCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            // await
        });
        AddCommand2 = ReactiveCommand.Create(() =>
        {
            Console.WriteLine("yahoo");
            // var item = new DataItemNode("new key", "value");
            // SubNodes.Add(item);
            // var 
            // Console.WriteLine($"Now adding a {node.Title} of type {_dataType}");
            
            
            // DataManager.Remove(assetName, _dataType);
        });
        
        AddCommand2 = ReactiveCommand.Create(() =>
        {
            Console.WriteLine("yahoo");
            // var item = new DataItemNode("new key", "value");
            // SubNodes.Add(item);
            // var 
            // Console.WriteLine($"Now adding a {node.Title} of type {_dataType}");
            
            
            // DataManager.Remove(assetName, _dataType);
        });
    }
}


public class IdNode : NodeBase
{
    public string Id { get; }
    
    public IdNode(string title = "", string? icon = null, string? id = null)
        : base(title, icon)
    {
        Id = id ?? Guid.NewGuid().ToString();
    }
    
    // public IdNode(string title, string icon, ObservableCollection<NodeBase> subNodes, string? id = null)
    //     : base(title, icon, subNodes)
    // {
    //     Id = id ?? Guid.NewGuid().ToString();
    // }
}

// Used only for the Tab Switcher
public class PageNode : NodeBase
{
    public EditorPageViewModel Page { get; }
    public PageNode Parent { get; set; }
    public bool IsTopLevel => Parent == null;

    public PageNode(EditorPageViewModel page, PageNode parent = null)
        : base(page.Title, page.Icon)
    {
        Page = page;
        Parent = parent;

        // // Listen for title changes on the page
        // page.PropertyChanged += (s, e) => 
        // {
        //     if (e.PropertyName == nameof(EditorPageViewModel.Title))
        //         OnPropertyChanged(nameof(Title));
        // };
    }
    
    public new string Title => Page.Title;
    
    public PageNode AddChild(EditorPageViewModel childPage)
    {
        var childNode = new PageNode(childPage, this);
        SubNodes.Add(childNode);
        return childNode;
    }
    
    public void RemoveChild(PageNode childNode)
    {
        SubNodes.Remove(childNode);
        childNode.Parent = null;
    }
}