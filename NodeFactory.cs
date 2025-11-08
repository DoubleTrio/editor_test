using System;
using System.Linq;
using AvaloniaTest.Services;
using AvaloniaTest.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaTest;

public class NodeFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NodeFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public T Create<T>(params object[] args)
        where T : NodeBase
    {
        var dialogService = _serviceProvider.GetService<IDialogService>()
                            ?? throw new InvalidOperationException("IDialogService needs to be registered.");

        var allArgs = new object[] { dialogService }.Concat(args).ToArray();
        return (T)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T), allArgs);
    }
    
    public OpenEditorNode CreateOpenEditorNode(string title, string? icon = null, string editorKey = "")
        => Create<OpenEditorNode>(title, icon, editorKey);

    public DataRootNode CreateDataRootNode(string dataType, string editorKey, string title, string? icon = null)
        => Create<DataRootNode>(dataType, editorKey, title, icon);

    public DataItemNode CreateDataItemNode(string key, string editorKey, string title, string? icon = null)
        => Create<DataItemNode>(key, editorKey, title, icon);

    public ActionDataNode CreateActionDataNode(string title, string? icon = null)
        => Create<ActionDataNode>(title, icon);

    public PageNode CreatePageNode(string title, string? icon = null, string editorKey = "")
        => Create<PageNode>(title, icon, editorKey);
    
}
