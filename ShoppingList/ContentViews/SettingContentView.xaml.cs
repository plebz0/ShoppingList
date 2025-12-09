namespace ShoppingList.ContentViews
{
    public partial class SettingContentView : ContentView
    {
        private string currentValue = string.Empty;

        public event EventHandler<(string OldValue, string NewValue)>? NameChanged;

        public event EventHandler<string>? DeleteClicked;

        public SettingContentView()
        {
            InitializeComponent();
        }

        public SettingContentView(string name) : this()
        {
            currentValue = name;
            NameEntry.Text = name;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            currentValue = BindingContext?.ToString() ?? string.Empty;
            NameEntry.Text = currentValue;
        }

        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            var newValue = e.NewTextValue?.Trim() ?? string.Empty;

            if (newValue == currentValue) return;

            NameChanged?.Invoke(this, (currentValue, newValue));
            currentValue = newValue;
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentValue)) return;
            DeleteClicked?.Invoke(this, currentValue);
        }
    }
}
