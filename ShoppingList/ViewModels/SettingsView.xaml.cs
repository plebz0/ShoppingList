using ShoppingList.Models;
using System.Collections.ObjectModel;
using System.Xml.Serialization;




namespace ShoppingList.Views;

public partial class SettingsView : ContentPage
{
    public ObservableCollection<string> CategoryItems { get; } = new();
    public ObservableCollection<string> StoreItems { get; } = new();

    public SettingsView()
    {
        InitializeComponent();
        BindingContext = this;

        LoadFromConfig();
    }

    private void LoadFromConfig()
    {
        CategoryItems.Clear();
        if (AppData.Config?.Categories != null)
        {
            foreach (var c in AppData.Config.Categories)
                CategoryItems.Add(c);
        }

        StoreItems.Clear();
        if (AppData.Config?.Stores != null)
        {
            foreach (var s in AppData.Config.Stores)
                StoreItems.Add(s);
        }
    }

    private void OnAddCategoryClicked(object sender, EventArgs e)
    {
        var text = CategoryEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return;

        if (!CategoryItems.Contains(text))
        {
            CategoryItems.Add(text);

            if (AppData.Config?.Categories != null && !AppData.Config.Categories.Contains(text))
            {
                AppData.Config.Categories.Add(text);
                AppData.Save();               
            }
        }

        CategoryEntry.Text = string.Empty;
    }

    private void OnAddStoreClicked(object sender, EventArgs e)
    {
        var text = StoreEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return;

        if (!StoreItems.Contains(text))
        {
            StoreItems.Add(text);

            if (AppData.Config?.Stores != null && !AppData.Config.Stores.Contains(text))
            {
                AppData.Config.Stores.Add(text);
                AppData.Save();

               
            }
        }

        StoreEntry.Text = string.Empty;
    }

  

    private void OnCategoryNameChanged(object? sender, (string OldValue, string NewValue) args)
    {
        var (oldName, newName) = args;
        if (string.IsNullOrWhiteSpace(newName) || oldName == newName) return;
        if (CategoryItems.Contains(newName)) return; // unikamy duplikat�w

        var idx = CategoryItems.IndexOf(oldName);
        if (idx >= 0)
            CategoryItems[idx] = newName;

        if (AppData.Config?.Categories != null)
        {
            var confIdx = AppData.Config.Categories.IndexOf(oldName);
            if (confIdx >= 0)
                AppData.Config.Categories[confIdx] = newName;
            else if (!AppData.Config.Categories.Contains(newName))
                AppData.Config.Categories.Add(newName);

            AppData.Save();

           
        }
    }

    private void OnStoreNameChanged(object? sender, (string OldValue, string NewValue) args)
    {
        var (oldName, newName) = args;
        if (string.IsNullOrWhiteSpace(newName) || oldName == newName) return;
        if (StoreItems.Contains(newName)) return; // unikamy duplikat�w

        var idx = StoreItems.IndexOf(oldName);
        if (idx >= 0)
            StoreItems[idx] = newName;

        if (AppData.Config?.Stores != null)
        {
            var confIdx = AppData.Config.Stores.IndexOf(oldName);
            if (confIdx >= 0)
                AppData.Config.Stores[confIdx] = newName;
            else if (!AppData.Config.Stores.Contains(newName))
                AppData.Config.Stores.Add(newName);

            AppData.Save();


        }
    }
    // -----------------------------
    // EXPORT
    // -----------------------------
    private async void OnExportClicked(object sender, EventArgs e)
    {
        try
        {
            // przygotuj dane do eksportu (TAK SAMO jak Save)
            var data = new AppData.PersistedData
            {
                Config = new AppData.PersistedConfig
                {
                    Categories = AppData.Config.Categories?.ToList() ?? new List<string>(),
                    Stores = AppData.Config.Stores?.ToList() ?? new List<string>()
                },
                Items = AppData.Items.Select(i => new AppData.PersistedItem
                {
                    Name = i.Name,
                    Unit = i.Unit,
                    Quantity = i.Quantity,
                    IsBought = i.IsBought,
                    Categories = i.Categories?.ToList() ?? new List<string>(),
                    Stores = i.Stores?.ToList() ?? new List<string>()
                }).ToList()
            };

            var serializer = new XmlSerializer(typeof(AppData.PersistedData));

            string fileName = $"shopping_export_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
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

    // -----------------------------
    // IMPORT
    // -----------------------------

    private static readonly FilePickerFileType XmlFileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.iOS, new[] { "public.xml" } },
        { DevicePlatform.MacCatalyst, new[] { "public.xml" } },
        { DevicePlatform.Android, new[] { "application/xml", "text/xml", ".xml" } },
        { DevicePlatform.WinUI, new[] { ".xml" } }
    });

    private async void OnImportClicked(object sender, EventArgs e)
    {
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Import Shopping XML File",
                FileTypes = XmlFileType
            });

            if (file == null)
                return;

            var serializer = new XmlSerializer(typeof(AppData.PersistedData));

            AppData.PersistedData? imported = null;

            using (var stream = await file.OpenReadAsync())
            {
                imported = serializer.Deserialize(stream) as AppData.PersistedData;
            }

            if (imported == null)
            {
                await DisplayAlert("Error", "Invalid XML file.", "OK");
                return;
            }

            bool replace = ReplaceImportCheckBox.IsChecked;

            if (replace)
                AppData.Items.Clear();

            foreach (var it in imported.Items ?? new List<AppData.PersistedItem>())
            {
                AppData.Items.Add(new ItemEntity
                {
                    Name = it.Name,
                    Unit = it.Unit,
                    Quantity = it.Quantity,
                    IsBought = it.IsBought,
                    Categories = new ObservableCollection<string>(it.Categories ?? new List<string>()),
                    Stores = new ObservableCollection<string>(it.Stores ?? new List<string>())
                });
            }

            // Zastępujemy również kategorie/stores z configu?
            if (replace)
            {
                AppData.Config.Categories.Clear();
                AppData.Config.Stores.Clear();

                foreach (var c in imported.Config?.Categories ?? new List<string>())
                    AppData.Config.Categories.Add(c);

                foreach (var s in imported.Config?.Stores ?? new List<string>())
                    AppData.Config.Stores.Add(s);
            }

            AppData.Save();
            MessagingCenter.Send(this, "ItemsUpdated");

            await DisplayAlert("Import Complete",
                replace ? "List replaced." : "Items added.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Import Error", ex.ToString(), "OK");
        }
    }

}
