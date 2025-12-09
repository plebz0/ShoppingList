using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShoppingList.Models
{
    public class ItemEntity : INotifyPropertyChanged
    {
        private string name;
        private double quantity;
        private string unit;
        private bool _isBought;
        private ObservableCollection<string> categories;
        private ObservableCollection<string> stores;

        public string Name
        {
            get => name;
            set { if (name != value) { name = value; OnPropertyChanged(); } }
        }

        public double Quantity
        {
            get => quantity;
            set { if (quantity != value) { quantity = value; OnPropertyChanged(); } }
        }

        public string Unit
        {
            get => unit;
            set { if (unit != value) { unit = value; OnPropertyChanged(); } }
        }

        public bool IsBought
        {
            get => _isBought;
            set { if (_isBought != value) { _isBought = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<string> Categories
        {
            get => categories;
            set { if (categories != value) { categories = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<string> Stores
        {
            get => stores;
            set { if (stores != value) { stores = value; OnPropertyChanged(); } }
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
