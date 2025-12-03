using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Maui.Controls;
using ShoppingList.Models;

namespace ShoppingList.ContentViews
{
    public partial class CategoryContentView : ContentView
    {
        public string CategoryName { get; private set; } = string.Empty;

        private ObservableCollection<ItemEntity> _itemsSource;

        public CategoryContentView(string category, ObservableCollection<ItemEntity> items)
        {
            InitializeComponent();
            CategoryName = category;
            CategoryLabel.Text = category;

            _itemsSource = items;
            _itemsSource.CollectionChanged += OnItemsChanged;

            ToggleButton.Text = "▲";
            BuildItems();
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(BuildItems);
        }

        private void BuildItems()
        {
            ItemsContainer.Children.Clear();

            var sorted = _itemsSource
                .OrderBy(i => i.IsBought);

            foreach (var it in sorted)
            {
                ItemsContainer.Children.Add(new ProductItemView { Item = it });
            }
        }

        private void OnToggleClicked(object sender, EventArgs e)
        {
            ItemsContainer.IsVisible = !ItemsContainer.IsVisible;
            ToggleButton.Text = ItemsContainer.IsVisible ? "▲" : "▼";
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent == null)
                _itemsSource.CollectionChanged -= OnItemsChanged;
        }
    }
}
