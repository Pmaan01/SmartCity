using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set { isBusy = value; OnPropertyChanged(); }
        }

        protected void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(name);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
