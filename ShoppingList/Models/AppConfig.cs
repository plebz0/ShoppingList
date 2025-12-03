using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.Models
{

    public class AppConfig
    {
        public ObservableCollection<string> Categories { get; set; } = new();
        public ObservableCollection<string> Stores { get; set; } = new();

        public AppConfig() { }
    }
}

