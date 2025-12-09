using Microsoft.Maui.Controls;
using ShoppingList.Models;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace ShoppingList.Views
{
    public partial class RecipesView : ContentPage
    {
        public ObservableCollection<Recipe> Recipes => RecipesStore.Recipes;
        public ObservableCollection<ItemEntity> NewIngredients { get; } = new();

        public RecipesView()
        {
            InitializeComponent();
            BindingContext = this;
        }


        private void OnImportRecipeClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Recipe recipe)
            {
                foreach (var i in recipe.Ingredients)
                {
                    AppData.Items.Add(new ItemEntity
                    {
                        Name = i.Name,
                        Unit = i.Unit,
                        Quantity = i.Quantity,
                        IsBought = false,
                        Categories = new ObservableCollection<string>(i.Categories ?? new ObservableCollection<string>()),
                        Stores = new ObservableCollection<string>(i.Stores ?? new ObservableCollection<string>())
                    });
                }

                AppData.Save();
            }
        }


        private void OnAddIngredientClicked(object sender, EventArgs e)
        {
            var name = NewIngName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return;

            if (!double.TryParse(NewIngQty.Text, out var qty))
                qty = 1;

            var unit = string.IsNullOrWhiteSpace(NewIngUnit.Text)
                ? "pcs"
                : NewIngUnit.Text.Trim();

            NewIngredients.Add(new ItemEntity
            {
                Name = name,
                Unit = unit,
                Quantity = qty,
                Categories = new ObservableCollection<string>(),
                Stores = new ObservableCollection<string>()
            });

            NewIngName.Text = string.Empty;
            NewIngQty.Text = string.Empty;
            NewIngUnit.Text = string.Empty;
        }

        private void OnRemoveNewIngredientClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is ItemEntity ing)
                NewIngredients.Remove(ing);
        }

        private void OnSaveRecipeClicked(object sender, EventArgs e)
        {
            var recipeName = NewRecipeName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(recipeName))
                return;

            var recipe = new Recipe { Name = recipeName, Description = NewRecipeDescription.Text };

            foreach (var i in NewIngredients)
            {
                recipe.Ingredients.Add(new ItemEntity
                {
                    Name = i.Name,
                    Unit = i.Unit,
                    Quantity = i.Quantity,
                    Categories = new ObservableCollection<string>(i.Categories ?? new ObservableCollection<string>()),
                    Stores = new ObservableCollection<string>(i.Stores ?? new ObservableCollection<string>())

                });
            }

            RecipesStore.Recipes.Add(recipe);

            NewRecipeName.Text = string.Empty;
            NewRecipeDescription.Text = string.Empty;
            NewIngredients.Clear();
        }

        private async void OnExportRecipeClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is not Button btn || btn.BindingContext is not Recipe recipe)
                    return;

                var data = new PersistedRecipe
                {
                    Name = recipe.Name,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients.Select(i => new AppData.PersistedItem
                    {
                        Name = i.Name,
                        Unit = i.Unit,
                        Quantity = i.Quantity,
                        IsBought = false,
                        Categories = i.Categories?.ToList() ?? new List<string>(),
                        Stores = i.Stores?.ToList() ?? new List<string>()
                    }).ToList()
                };

                var serializer = new XmlSerializer(typeof(PersistedRecipe));

                string fileName = $"{SanitizeFileName(recipe.Name)}_recipe_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
                string folder = FileSystem.AppDataDirectory;
                string fullPath = Path.Combine(folder, fileName);

                using (var stream = File.Create(fullPath))
                    serializer.Serialize(stream, data);

                await DisplayAlert("Export Complete",
                    $"Saved to:\n{fullPath}\n\nYou can now copy the file manually.",
                    "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Export Error", ex.ToString(), "OK");
            }
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        private static readonly FilePickerFileType XmlFileType = new(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xml" } },
                { DevicePlatform.MacCatalyst, new[] { "public.xml" } },
                { DevicePlatform.Android, new[] { "application/xml", "text/xml", ".xml" } },
                { DevicePlatform.WinUI, new[] { ".xml" } }
            });

        private async void OnImportRecipeFileClicked(object sender, EventArgs e)
        {
            try
            {
                var file = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Import Recipe XML File",
                    FileTypes = XmlFileType
                });

                if (file == null)
                    return;

                var serializer = new XmlSerializer(typeof(PersistedRecipe));

                PersistedRecipe? imported = null;

                using (var stream = await file.OpenReadAsync())
                    imported = serializer.Deserialize(stream) as PersistedRecipe;

                if (imported == null)
                {
                    await DisplayAlert("Import Failed", "Invalid recipe XML file.", "OK");
                    return;
                }

                var recipe = new Recipe
                {
                    Name = imported.Name ?? "Unnamed Recipe",
                    Description = imported.Description ?? ""
                };

                foreach (var it in imported.Ingredients ?? new List<AppData.PersistedItem>())
                {
                    recipe.Ingredients.Add(new ItemEntity
                    {
                        Name = it.Name,
                        Unit = it.Unit,
                        Quantity = it.Quantity,
                        IsBought = false,
                        Categories = new ObservableCollection<string>(it.Categories ?? new List<string>()),
                        Stores = new ObservableCollection<string>(it.Stores ?? new List<string>())
                    });
                }

                RecipesStore.Recipes.Add(recipe);

                await DisplayAlert("Import Complete", $"Imported recipe:\n{recipe.Name}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Import Error", ex.ToString(), "OK");
            }
        }
    }

   
    public class PersistedRecipe
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        [XmlArray("Ingredients"), XmlArrayItem("Item")]
        public List<AppData.PersistedItem>? Ingredients { get; set; }
    }
}
