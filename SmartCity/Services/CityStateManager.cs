using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SmartCity.Services
{

    public class CityStateManager : INotifyPropertyChanged
    {
        private static readonly Lazy<CityStateManager> _instance = new(() => new CityStateManager());
        public static CityStateManager Instance => _instance.Value;

        private string _selectedCity = "Vancouver";

        public string SelectedCity
        {
            get => _selectedCity;
            set
            {
                if (!Equals(_selectedCity, value))
                {
                    _selectedCity = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}