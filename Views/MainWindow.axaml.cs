using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        RoleAutoCompleteBox.AddHandler(
            PointerPressedEvent,
            (sender, e) =>
                
            {
 
     
               var src = (e.Source as Visual);
       
        
               while (src != null)
               {
              
                   if (src is Button && src.Name == "AutoCompleteBoxClearButton") 
                       return; 
                   src = src.GetVisualParent();
               }
       
          
                Dispatcher.UIThread.Post(() => { RoleAutoCompleteBox.IsDropDownOpen = true; });
            },
            Avalonia.Interactivity.RoutingStrategies.Tunnel // use tunneling
        );
        
 
        
        
        RoleAutoCompleteBox.AddHandler(
            AutoCompleteBox.KeyDownEvent,
            (sender, e) =>
            {
   
                if (DataContext is MainWindowViewModel vm) {
                    if (e.Key == Avalonia.Input.Key.Enter)
                    {
                        if (RoleAutoCompleteBox.SelectedItem != null)
                        {
                            vm.RoleBinder.CommitSelection((string)RoleAutoCompleteBox.SelectedItem);
                        }
                        else
                        {
                            var itemToCommit = RoleAutoCompleteBox.ItemsSource
                                .OfType<string>() // cast to T safely
                                .FirstOrDefault(x => x != null && x.ToString().StartsWith(RoleAutoCompleteBox.SearchText, StringComparison.InvariantCultureIgnoreCase));

                            if (itemToCommit != null)
                            {
                                vm.RoleBinder.CommitSelection(itemToCommit); 
                            }

                        
                        }
                    }
                }
                
            },
            Avalonia.Interactivity.RoutingStrategies.Tunnel
        );
        
        
        RoleAutoCompleteBox.DropDownClosed += (_, __) =>
        {

            if (DataContext is MainWindowViewModel vm)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    
                    if (vm.RoleBinder.suppressRevert)
                    {
                        vm.RoleBinder.suppressRevert = false;
                    }

                    if (vm.RoleBinder.HasMatchStart)
                    {
                        vm.RoleBinder.RevertIfInvalid();
                    }



                    
                    if (RoleAutoCompleteBox.Text != "") { 
                        var len = RoleAutoCompleteBox.Text.Length;
                        RoleAutoCompleteBox.CaretIndex = len;
                    }

                }, DispatcherPriority.Background);
            }
           
        };
        
       
        
        
    }
    
    // public void ButtonClicked(object source, RoutedEventArgs args)
    // {
    //     if (Double.TryParse(celsius.Text, out double C))
    //     {
    //         var F = C * (9d / 5d) + 32;
    //         fahrenheit.Text = F.ToString("0.0");
    //     }
    //     else
    //     {
    //         celsius.Text = "0";
    //         fahrenheit.Text = "0";
    //     }
    // }
    //
    //
    // private void Celsius_OnKeyDown(object? sender, KeyEventArgs e)
    // {
    //     Debug.WriteLine(e.Key);
    //     if (double.TryParse(celsius.Text, out double C))
    //     {
    //         Debug.WriteLine($"Celsius: {C}");
    //         var F = C * (9d / 5d) + 32;
    //         fahrenheit.Text = F.ToString("0.00");
    //     }
    //     else
    //     {
    //         celsius.Text = "0";
    //         fahrenheit.Text = "0";
    //     }
    // }
    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var results = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            // Title = "Open .rsground File",
            // SuggestedStartLocation = await form.GroundEditForm.StorageProvider.TryGetFolderFromPathAsync(mapDir),
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Ground Files")
                {
                    Patterns = new[] { "*." +　"json" }
                }
            }
        });

        if (results != null && results.Count > 0)
        {
            var selected = results[0];
            // NAME: apple_woods_secondary.json
            // Path: file:///Users/thekacefiles/Coding/PMD/PMDODump/DumpAsset/Data/AutoTile/apple_woods_secondary.json
            // Path.AbsolutePath: /Users/thekacefiles/Coding/PMD/PMDODump/DumpAsset/Data/AutoTile/apple_woods_secondary.json
            // Path.AbsolutePath: 
            // selected.
            Console.WriteLine(selected.Path.AbsolutePath);

        }
    }
}