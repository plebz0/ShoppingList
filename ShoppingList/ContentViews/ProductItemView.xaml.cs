using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Maui.Controls;
using ShoppingList.Models;
using ShoppingList.ContentViews;

namespace ShoppingList.ContentViews
{
    public partial class ProductItemView : ContentView
    {
        public static readonly BindableProperty ItemProperty =
            BindableProperty.Create(nameof(Item), typeof(ItemEntity), typeof(ProductItemView), propertyChanged: OnItemChanged);

        public ItemEntity Item
        {
            get => (ItemEntity)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        static readonly List<string> defaultUnits = new() { "pcs", "kg", "g", "l", "ml" };

        public ProductItemView()
        {
            InitializeComponent();

            UnitPicker.ItemsSource = defaultUnits;
            BindingContext = this;
        }

        static void OnItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (ProductItemView)bindable;
            view.RefreshFromItem();
        }

        void RefreshFromItem()
        {
            if (Item == null)
            {
                NameEntry.Text = string.Empty;
                QtyEntry.Text = "0";
                UnitPicker.SelectedItem = null;
                PrimaryCategoryLabel.Text = string.Empty;
                PrimaryStoreLabel.Text = string.Empty;
                return;
            }

            NameEntry.Text = Item.Name;
            QtyEntry.Text = Item.Quantity.ToString(CultureInfo.CurrentCulture);

            UnitPicker.SelectedItem =
                string.IsNullOrEmpty(Item.Unit) ? defaultUnits[0] : Item.Unit;

            PrimaryCategoryLabel.Text =
                Item.Categories != null && Item.Categories.Any()
                ? string.Join(", ", Item.Categories)
                : "(none)";

            PrimaryStoreLabel.Text =
                Item.Stores != null && Item.Stores.Any()
                ? string.Join(", ", Item.Stores)
                : "(none)";

            UpdateVisualForBought(Item.IsBought);
        }


        void UpdateVisualForBought(bool isBought)
        {
            Opacity = isBought ? 0.6 : 1.0;
        }

        private void OnIncreaseClicked(object sender, EventArgs e)
        {
            if (Item == null) return;
            Item.Quantity += 1;
            QtyEntry.Text = Item.Quantity.ToString(CultureInfo.CurrentCulture);
            AppData.Save();
        }

        private void OnDecreaseClicked(object sender, EventArgs e)
        {
            if (Item == null) return;
            if (Item.Quantity > 0) Item.Quantity -= 1;
            QtyEntry.Text = Item.Quantity.ToString(CultureInfo.CurrentCulture);
            AppData.Save();
        }

        private void OnQtyEntryCompleted(object sender, EventArgs e)
        {
            if (Item == null) return;
            if (double.TryParse(QtyEntry.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var v))
            {
                Item.Quantity = v;
                QtyEntry.Text = Item.Quantity.ToString(CultureInfo.CurrentCulture);
                AppData.Save();
            }
            else
            {
                QtyEntry.Text = Item.Quantity.ToString(CultureInfo.CurrentCulture);
            }
        }

        private async void OnEditCategoriesClicked(object sender, EventArgs e)
        {
            if (Item == null) return;
            var options = AppData.Config?.Categories ?? new System.Collections.ObjectModel.ObservableCollection<string>();
            var page = new ContentViews.MultiSelectPage("Select categories", options, Item.Categories);
            await Navigation.PushModalAsync(page);
            RefreshFromItem();
        }

        private async void OnEditStoresClicked(object sender, EventArgs e)
        {
            if (Item == null) return;
            var options = AppData.Config?.Stores ?? new System.Collections.ObjectModel.ObservableCollection<string>();
            var page = new ContentViews.MultiSelectPage("Select stores", options, Item.Stores);
            await Navigation.PushModalAsync(page);
            RefreshFromItem();
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
          
            if (Item == null) return;
            AppData.Items.Remove(Item);
            AppData.Save();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (Item != null)
            {
                Item.Categories.CollectionChanged += (_, __) => RefreshFromItem();
                Item.Stores.CollectionChanged += (_, __) => RefreshFromItem();
            }
        }

        private void OnNameEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Item == null) return;
            Item.Name = e.NewTextValue;
        }

        private void OnNameEntryCompleted(object sender, EventArgs e)
        {
            if (Item == null) return;
            Item.Name = NameEntry.Text?.Trim() ?? "";
            AppData.Save();
        }

    }
}   