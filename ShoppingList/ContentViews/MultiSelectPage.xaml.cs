using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using ShoppingList.Models;

namespace ShoppingList.ContentViews
{
    public partial class MultiSelectPage : ContentPage
    {
        public class OptionItem
        {
            public string Name { get; set; } = string.Empty;
            public bool IsSelected { get; set; }
        }

        readonly ObservableCollection<OptionItem> options = new();

        readonly ObservableCollection<string> targetCollection;

        public MultiSelectPage(string title, IEnumerable<string> availableOptions, ObservableCollection<string> currentSelected)
        {
            InitializeComponent();
            TitleLabel.Text = title;
            targetCollection = currentSelected;

            var distinct = availableOptions?.Distinct() ?? Enumerable.Empty<string>();
            foreach (var o in distinct)
            {
                options.Add(new OptionItem { Name = o, IsSelected = currentSelected?.Contains(o) == true });
            }

            OptionsList.ItemsSource = options;
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnOk(object sender, EventArgs e)
        {
            // Apply selection to the target ObservableCollection (preserve object and notify)
            targetCollection.Clear();
            foreach (var o in options.Where(x => x.IsSelected).Select(x => x.Name))
                targetCollection.Add(o);

            AppData.Save();
            await Navigation.PopModalAsync();
        }
    }
}