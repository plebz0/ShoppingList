using ShoppingList.Models;
using ShoppingList.Views;

namespace ShoppingList
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RecipesView), typeof(RecipesView));
            Routing.RegisterRoute(nameof(ShoppingListView), typeof(ShoppingListView));
            Routing.RegisterRoute(nameof(SettingsView), typeof(SettingsView));

            AppData.Load();
        }
    }
}
