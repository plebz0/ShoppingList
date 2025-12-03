using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using ShoppingList.Models;

namespace ShoppingList.Views
{
    public partial class ShoppingListView : ContentPage
    {
        public ObservableCollection<FilterItem> CategoryFilters { get; } = new();
        public ObservableCollection<FilterItem> StoreFilters { get; } = new();

        private ObservableCollection<string> selectedCategories = new();
        private ObservableCollection<string> selectedStores = new();

        public ShoppingListView()
        {
            InitializeComponent();
            BindingContext = this;

            InitFilters();
            InitAddControls();
            UpdateSelectedLabels();
            RebuildGroupedItems();

            // settings updated
            MessagingCenter.Subscribe<SettingsView>(this, "CategoriesUpdated", sender =>
            {
                Dispatcher.Dispatch(() =>
                {
                    InitFilters();
                    RebuildGroupedItems();
                });
            });

            MessagingCenter.Subscribe<SettingsView>(this, "StoresUpdated", sender =>
            {
                Dispatcher.Dispatch(() =>
                {
                    InitFilters();
                    RebuildGroupedItems();
                });
            });

            MessagingCenter.Subscribe<SettingsView>(this, "ItemsUpdated", sender =>
            {
                Dispatcher.Dispatch(() =>
                {
                    RebuildGroupedItems();
                });
            });


            // defaults
            GroupingPicker.SelectedIndex = 0;
            SortPicker.SelectedIndex = 0;

            foreach (var it in AppData.Items)
                it.PropertyChanged += (_, __) => RebuildGroupedItems();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateSelectedLabels();
        }

        // =========================
        // INIT
        // =========================
        private void InitFilters()
        {
            CategoryFilters.Clear();
            StoreFilters.Clear();

            if (AppData.Config?.Categories != null)
            {
                foreach (var c in AppData.Config.Categories.Distinct())
                    CategoryFilters.Add(new FilterItem { Name = c, IsSelected = false });
            }

            if (AppData.Config?.Stores != null)
            {
                foreach (var s in AppData.Config.Stores.Distinct())
                    StoreFilters.Add(new FilterItem { Name = s, IsSelected = false });
            }
        }

        private void InitAddControls()
        {
            UnitPicker.Items.Clear();
            UnitPicker.Items.Add("pices");
            UnitPicker.Items.Add("kg");
            UnitPicker.Items.Add("g");
            UnitPicker.Items.Add("l");
            UnitPicker.Items.Add("ml");
            UnitPicker.SelectedIndex = 0;

            QuantityEntry.Text = "1";
            QuantityStepper.Value = 1;
        }

        private void UpdateSelectedLabels()
        {
            SelectedCategoriesLabel.Text =
                selectedCategories.Any() ? string.Join(", ", selectedCategories) : "(none)";

            SelectedStoresLabel.Text =
                selectedStores.Any() ? string.Join(", ", selectedStores) : "(none)";
        }

        // =========================
        // MULTISELECT HANDLERS
        // =========================
        private async void OnSelectCategoriesClicked(object sender, EventArgs e)
        {
            var options = AppData.Config?.Categories ?? new ObservableCollection<string>();
            var page = new ContentViews.MultiSelectPage("Select categories", options, selectedCategories);

            await Navigation.PushModalAsync(page);

            UpdateSelectedLabels();
            RebuildGroupedItems();
        }

        private async void OnSelectStoresClicked(object sender, EventArgs e)
        {
            var options = AppData.Config?.Stores ?? new ObservableCollection<string>();
            var page = new ContentViews.MultiSelectPage("Select stores", options, selectedStores);

            await Navigation.PushModalAsync(page);

            UpdateSelectedLabels();
            RebuildGroupedItems();
        }

        // =========================
        // SORTING + FILTERING
        // =========================
        private ObservableCollection<ItemEntity> ApplySorting(ObservableCollection<ItemEntity> items)
        {
            int sort = SortPicker.SelectedIndex;

            var sorted = sort switch
            {
                0 => items.OrderBy(i => i.Name),
                1 => items.OrderByDescending(i => i.Name),
                2 => items.OrderBy(i => i.Quantity),
                3 => items.OrderByDescending(i => i.Quantity),
                _ => items.OrderBy(i => i.Name)
            };

            return new ObservableCollection<ItemEntity>(sorted);
        }

        private ObservableCollection<ItemEntity> ApplyFilters()
        {
            var items = AppData.Items.AsEnumerable();

            var selectedCats = CategoryFilters.Where(f => f.IsSelected).Select(f => f.Name).ToList();
            if (selectedCats.Any())
            {
                items = items.Where(i =>
                    i.Categories != null &&
                    i.Categories.Any(c => selectedCats.Contains(c)));
            }

            var selectedStoreFilters = StoreFilters.Where(f => f.IsSelected).Select(f => f.Name).ToList();
            if (selectedStoreFilters.Any())
            {
                items = items.Where(i =>
                    i.Stores != null &&
                    i.Stores.Any(s => selectedStoreFilters.Contains(s)));
            }

            if (ShowPurchasedCheckBox?.IsChecked == false)
                items = items.Where(i => !i.IsBought);

            return new ObservableCollection<ItemEntity>(items);
        }

        // =========================
        // GROUPING + RENDER
        // =========================
        private void RebuildGroupedItems()
        {
            try
            {
                Dispatcher.Dispatch(() =>
                {
                    CategoriesContainer.Children.Clear();

                    var filtered = ApplySorting(ApplyFilters());

                    if (filtered.Count == 0)
                        return;

                    int grouping = GroupingPicker.SelectedIndex;

                    // NONE
                    if (grouping == 0)
                    {
                        CategoriesContainer.Children.Add(
                            new ContentViews.CategoryContentView(
                                "All Items",
                                filtered
                            )
                        );
                        return;
                    }

                    // CATEGORY
                    if (grouping == 1)
                    {
                        foreach (var cat in AppData.Config.Categories.Distinct())
                        {
                            var items = new ObservableCollection<ItemEntity>(
                                filtered.Where(i => i.Categories.Contains(cat))
                            );

                            if (items.Any())
                                CategoriesContainer.Children.Add(
                                    new ContentViews.CategoryContentView(cat, items)
                                );
                        }
                        return;
                    }

                    // STORE
                    if (grouping == 2)
                    {
                        foreach (var store in AppData.Config.Stores.Distinct())
                        {
                            var items = new ObservableCollection<ItemEntity>(
                                filtered.Where(i => i.Stores.Contains(store))
                            );

                            if (items.Any())
                                CategoriesContainer.Children.Add(
                                    new ContentViews.CategoryContentView(store, items)
                                );
                        }
                        return;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Grouping error: {ex}");
                CategoriesContainer.Children.Add(
                    new ContentViews.CategoryContentView(
                        "Grouping failed",
                        new ObservableCollection<ItemEntity>(AppData.Items)
                    )
                );
            }
        }

        // =========================
        // EVENT HANDLERS
        // =========================
        private void OnSortChanged(object sender, EventArgs e)
        {
            RebuildGroupedItems();
        }

        private void OnQuantityStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            QuantityEntry.Text = e.NewValue.ToString(CultureInfo.CurrentCulture);
        }

        private void OnQuantityEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(e.NewTextValue, NumberStyles.Number, CultureInfo.CurrentCulture, out var val))
            {
                val = Math.Clamp(val, QuantityStepper.Minimum, QuantityStepper.Maximum);
                QuantityStepper.Value = val;
            }
        }

        private void OnQuantityEntryCompleted(object sender, EventArgs e)
        {
            if (!double.TryParse(QuantityEntry.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out _))
                QuantityEntry.Text = QuantityStepper.Value.ToString(CultureInfo.CurrentCulture);
        }

        private void OnToggleCategoryFilterClicked(object sender, EventArgs e)
        {
            CategoryFilterPanel.IsVisible = !CategoryFilterPanel.IsVisible;
        }

        private void OnToggleStoreFilterClicked(object sender, EventArgs e)
        {
            StoreFilterPanel.IsVisible = !StoreFilterPanel.IsVisible;
        }

        private void OnCategoryFilterChanged(object sender, CheckedChangedEventArgs e)
        {
            RebuildGroupedItems();
        }

        private void OnStoreFilterChanged(object sender, CheckedChangedEventArgs e)
        {
            RebuildGroupedItems();
        }

        private void OnGroupingChanged(object sender, EventArgs e)
        {
            RebuildGroupedItems();
        }

        private void OnShowPurchasedChanged(object sender, CheckedChangedEventArgs e)
        {
            RebuildGroupedItems();
        }

        private void OnAddItemClicked(object sender, EventArgs e)
        {
            var name = newItemName.Text;
            if (string.IsNullOrWhiteSpace(name))
                return;

            var unit = UnitPicker.SelectedItem as string ?? "pcs";

            double quantity = 1;
            if (!string.IsNullOrWhiteSpace(QuantityEntry.Text) &&
                double.TryParse(QuantityEntry.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
                quantity = parsed;
            else
                quantity = QuantityStepper.Value;

            var item = new ItemEntity
            {
                Name = name.Trim(),
                Unit = unit,
                Quantity = quantity,
                IsBought = false,
                Categories = new ObservableCollection<string>(selectedCategories),
                Stores = new ObservableCollection<string>(selectedStores)
            };

            AppData.Items.Add(item);
            item.PropertyChanged += (_, __) => RebuildGroupedItems();
            AppData.Save();

            // reset
            newItemName.Text = string.Empty;
            QuantityEntry.Text = "1";
            QuantityStepper.Value = 1;

            selectedCategories.Clear();
            selectedStores.Clear();
            UpdateSelectedLabels();

            RebuildGroupedItems();
        }

        private void OnItemBoughtChanged(object sender, CheckedChangedEventArgs e)
        {
            AppData.Save();
            RebuildGroupedItems();
        }

        private void OnDeleteItemClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is ItemEntity item)
            {
                AppData.Items.Remove(item);
                AppData.Save();
                RebuildGroupedItems();
            }
        }
    }
}
