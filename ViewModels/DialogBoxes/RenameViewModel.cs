using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace AvaloniaTest.ViewModels
{
    public class RenameViewModel : ViewModelBase
    {
        public RenameViewModel()
        {
            Name = "";
        }

        private string name;
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

    }
}
