using System.Collections.ObjectModel;
using ShoppingList.Models;

namespace ShoppingList.Views
{
    public class ItemGroup : ObservableCollection<ItemEntity>
    {
        public string Key { get; }

        public ItemGroup(string key, IEnumerable<ItemEntity> items) : base(items)
        {
            Key = key;
        }
    }

    public class FilterItem
    {
        public string Name { get; set; } = "";
        public bool IsSelected { get; set; }
    }
}
