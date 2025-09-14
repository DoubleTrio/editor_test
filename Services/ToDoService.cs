using System.Collections.Generic;
using AvaloniaTest.DataModel;

namespace AvaloniaTest.Services
{
    public class ToDoService
    {
        public IEnumerable<ToDoItem> GetItems() => new[]
        {
            new ToDoItem { Description = "Walk the dog" },
            new ToDoItem { Description = "Buy some milk" },
            new ToDoItem { Description = "Learn Avalonia", IsChecked = true },
        };
    }
}