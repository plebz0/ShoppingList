using System.Collections.ObjectModel;

namespace ShoppingList.Models
{
    public class Recipe
    {
        public string Name { get; set; } = "";
        public ObservableCollection<ItemEntity> Ingredients { get; } = new();
    }

    public static class RecipesStore
    {
        public static ObservableCollection<Recipe> Recipes { get; } = new();

        static RecipesStore()
        {
            var pancakes = new Recipe { Name = "Pancakes" };
            pancakes.Ingredients.Add(new ItemEntity
            {
                Name = "Flour",
                Unit = "g",
                Quantity = 200,
                Categories = new ObservableCollection<string>(),
                Stores = new ObservableCollection<string>()
            });
            pancakes.Ingredients.Add(new ItemEntity
            {
                Name = "Milk",
                Unit = "ml",
                Quantity = 300,
                Categories = new ObservableCollection<string>(),
                Stores = new ObservableCollection<string>()
            });
            pancakes.Ingredients.Add(new ItemEntity
            {
                Name = "Egg",
                Unit = "pices",
                Quantity = 2,
                Categories = new ObservableCollection<string>(),
                Stores = new ObservableCollection<string>()
            });

            var tomatoPasta = new Recipe { Name = "Tomato Pasta" };
            tomatoPasta.Ingredients.Add(new ItemEntity
            {
                Name = "Pasta",
                Unit = "g",
                Quantity = 250,
                Categories = new ObservableCollection<string>(),
                Stores = new ObservableCollection<string>()
            });
            tomatoPasta.Ingredients.Add(new ItemEntity
            {
                Name = "Tomato sauce",
                Unit = "ml",
                Quantity = 300,
                Categories = new ObservableCollection<string>(),
                Stores = new ObservableCollection<string>()
            });
            tomatoPasta.Ingredients.Add(new ItemEntity
            {
                Name = "Olive oil",
                Unit = "ml",
                Quantity = 1,
                Categories = new ObservableCollection<string>(),
                Stores = new ObservableCollection<string>()
            });

            Recipes.Add(pancakes);
            Recipes.Add(tomatoPasta);
        }
    }
}
