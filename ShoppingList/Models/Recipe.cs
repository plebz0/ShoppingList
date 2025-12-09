using System.Collections.ObjectModel;

namespace ShoppingList.Models
{
    public class Recipe
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public ObservableCollection<ItemEntity> Ingredients { get; } = new();
    }

    public static class RecipesStore
    {
        public static ObservableCollection<Recipe> Recipes { get; } = new();

        static RecipesStore()
        {
            var pancakes = new Recipe
            {
                Name = "Pancakes",
                Description = "Description: Simple, classic pancakes made from a basic batter of flour, milk, and egg for a light, fluffy breakfast.\n\nRecipe: Mix flour, milk, and egg into a smooth batter, pour onto a hot pan, and cook until golden on both sides."
            };
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
                Unit = "pcs",
                Quantity = 2,
                Categories = new ObservableCollection<string>(),
                Stores = new ObservableCollection<string>()
            });

            var tomatoPasta = new Recipe
            {
                Name = "Tomato Pasta",
                Description = "Description: A quick, minimalist tomato pasta made with just pasta, tomato sauce, and olive oil for a simple, comforting meal.\n\nRecipe: Cook pasta, heat tomato sauce with a splash of olive oil, then combine and drizzle with a bit more olive oil before serving."
            };
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
