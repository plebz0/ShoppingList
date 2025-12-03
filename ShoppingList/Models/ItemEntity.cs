using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShoppingList.Models
{
    public class ItemEntity : INotifyPropertyChanged
    {
        private string _name;
        private double _quantity;
        private string _unit;
        private bool _isBought;
        private ObservableCollection<string> _categories;
        private ObservableCollection<string> _stores;

        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        public double Quantity
        {
            get => _quantity;
            set { if (_quantity != value) { _quantity = value; OnPropertyChanged(); } }
        }

        public string Unit
        {
            get => _unit;
            set { if (_unit != value) { _unit = value; OnPropertyChanged(); } }
        }

        public bool IsBought
        {
            get => _isBought;
            set { if (_isBought != value) { _isBought = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<string> Categories
        {
            get => _categories;
            set { if (_categories != value) { _categories = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<string> Stores
        {
            get => _stores;
            set { if (_stores != value) { _stores = value; OnPropertyChanged(); } }
        }

        public string PrimaryCategory => Categories?.FirstOrDefault() ?? "";
        public string PrimaryStore => Stores?.FirstOrDefault() ?? "";

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
