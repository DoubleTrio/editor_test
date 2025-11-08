using System;
using System.Collections.Generic;
using AvaloniaTest.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaTest;


public class DesignServiceProvider : IServiceProvider
{
    private readonly IServiceProvider _serviceProvider;

    public DesignServiceProvider()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        _serviceProvider = collection.BuildServiceProvider();
        _serviceProvider.RegisterPages();
    }

    public object? GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType);
    }
    
    public T? GetService<T>()
    {
        return _serviceProvider.GetService<T>();
    }
    
    public T GetRequiredService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}



public class PageFactory
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<string, Type> _map = new();

    public PageFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void Register<TPage>(string key) where TPage : EditorPageViewModel
    {
        _map[key] = typeof(TPage);
    }

    // public EditorPageViewModel? CreatePage(OpenEditorNode node)
    // {
    //     if (_map.TryGetValue(node.EditorKey, out var type))
    //     {
    //         return (EditorPageViewModel)_provider.GetRequiredService(type);
    //     }
    //
    //     return null;
    // }
    
    public EditorPageViewModel? CreatePage(string key)
    {
        if (_map.TryGetValue(key, out var type))
        {
            return (EditorPageViewModel)_provider.GetRequiredService(type);
        }

        return null;
    }
}


