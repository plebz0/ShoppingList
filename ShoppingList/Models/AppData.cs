using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Maui.Storage;

namespace ShoppingList.Models
{
    public static class AppData
    {
        public static readonly ObservableCollection<ItemEntity> Items = new();
        public static readonly AppConfig Config = new();

        public static void Save()
        {
          
            try
            {
                var dir = FileSystem.AppDataDirectory;
                Directory.CreateDirectory(dir);

                var path = Path.Combine(dir, "ShoppingListData.xml");

                var data = new PersistedData
                {
                    Config = new PersistedConfig
                    {
                        Categories = Config.Categories?.ToList() ?? new List<string>(),
                        Stores = Config.Stores?.ToList() ?? new List<string>()
                    },
                    Items = Items.Select(i => new PersistedItem
                    {
                        Name = i.Name,
                        Unit = i.Unit,
                        Quantity = i.Quantity,
                        IsBought = i.IsBought,
                        Categories = i.Categories?.ToList() ?? new List<string>(),
                        Stores = i.Stores?.ToList() ?? new List<string>()
                    }).ToList()
                };

                var serializer = new XmlSerializer(typeof(PersistedData));
                using var stream = File.Create(path);
                serializer.Serialize(stream, data);
            }
            catch (Exception)
            {
                Console.WriteLine("Error saving app data.");
            }
        }

        public static void Load()
        {
            var dir = FileSystem.AppDataDirectory;
            var path = Path.Combine(dir, "ShoppingListData.xml");

            bool loadedFromFile = false;

            if (File.Exists(path))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(PersistedData));
                    using var stream = File.OpenRead(path);
                    if (serializer.Deserialize(stream) is PersistedData data)
                    {
                       
                        Config.Categories.Clear();
                        if (data.Config?.Categories != null)
                        {
                            foreach (var c in data.Config.Categories)
                                Config.Categories.Add(c);
                        }

                        Config.Stores.Clear();
                        if (data.Config?.Stores != null)
                        {
                            foreach (var s in data.Config.Stores)
                                Config.Stores.Add(s);
                        }

                     
                        Items.Clear();
                        if (data.Items != null)
                        {
                            foreach (var it in data.Items)
                            {
                                Items.Add(new ItemEntity
                                {
                                    Name = it.Name,
                                    Unit = it.Unit,
                                    Quantity = it.Quantity,
                                    IsBought = it.IsBought,
                                    Categories = new ObservableCollection<string>(it.Categories ?? new List<string>()),
                                    Stores = new ObservableCollection<string>(it.Stores ?? new List<string>())
                                });
                            }
                        }

                        loadedFromFile = true;
                    }
                }
                catch (Exception)
                {
                    
                    loadedFromFile = false;
                }
            }

            if (loadedFromFile)
                return;

           
            Config.Categories.Clear();
            Config.Categories.Add("Dairy");
            Config.Categories.Add("Vegetables");
            Config.Categories.Add("Fruits");
            Config.Categories.Add("Meat");
            Config.Categories.Add("Snacks");

            Config.Stores.Clear();
            Config.Stores.Add("Biedronka");
            Config.Stores.Add("Lidl");
            Config.Stores.Add("Carrefour");
            Config.Stores.Add("Selgros");

            Items.Clear();
            Items.Add(new ItemEntity
            {
                Name = "Milk",
                Unit = "l",
                Quantity = 2,
                IsBought = false,
                Categories = new ObservableCollection<string> { "Dairy" },
                Stores = new ObservableCollection<string> { "Biedronka" }
            });

            Items.Add(new ItemEntity
            {
                Name = "Tomatoes",
                Unit = "kg",
                Quantity = 1,
                IsBought = false,
                Categories = new ObservableCollection<string> { "Vegetables" },
                Stores = new ObservableCollection<string> { "Lidl" }
            });
            Items.Add(new ItemEntity
            {
                Name = "Twix",
                Unit = "pices",
                Quantity = 1,
                IsBought = false,
                Categories = new ObservableCollection<string> { "Snacks" },
                Stores = new ObservableCollection<string> { "Selgros" }
            });
            Items.Add(new ItemEntity
            {
                Name = "Piwo",
                Unit = "l",
                Quantity = 1,
                IsBought = false,
                Categories = new ObservableCollection<string> { "Drink", "Alcohol" },
                Stores = new ObservableCollection<string> { "Selgros" }
            });
            Items.Add(new ItemEntity
            {
                Name = "Ice Cream",
                Unit = "l",
                Quantity = 1,
                IsBought = false,
                Categories = new ObservableCollection<string> { "Dairy", "Snacks" },
                Stores = new ObservableCollection<string> { "Selgros" }
            });
        }

        
        [XmlRoot("AppData")]
        public class PersistedData
        {
            public PersistedConfig? Config { get; set; }
            [XmlArray("Items"), XmlArrayItem("Item")]
            public List<PersistedItem>? Items { get; set; }
        }

        public class PersistedConfig
        {
            [XmlArray("Categories"), XmlArrayItem("Category")]
            public List<string>? Categories { get; set; }

            [XmlArray("Stores"), XmlArrayItem("Store")]
            public List<string>? Stores { get; set; }
        }

        public class PersistedItem
        {
            public string? Name { get; set; }
            public string? Unit { get; set; }
            public double Quantity { get; set; }
            public bool IsBought { get; set; }

            [XmlArray("Categories"), XmlArrayItem("Category")]
            public List<string>? Categories { get; set; }

            [XmlArray("Stores"), XmlArrayItem("Store")]
            public List<string>? Stores { get; set; }
        }
    }
}
