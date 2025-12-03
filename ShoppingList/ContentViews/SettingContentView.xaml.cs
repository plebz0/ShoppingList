namespace ShoppingList.ContentViews
{
    public partial class SettingContentView : ContentView
    {
        private string _currentValue = string.Empty;

        // event: nazwa zmieniona (old, new)
        public event EventHandler<(string OldValue, string NewValue)>? NameChanged;

        // event: klikni�to delete
        public event EventHandler<string>? DeleteClicked;

        public SettingContentView()
        {
            InitializeComponent();
        }

        public SettingContentView(string name) : this()
        {
            _currentValue = name;
            NameEntry.Text = name;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            _currentValue = BindingContext?.ToString() ?? string.Empty;
            NameEntry.Text = _currentValue;
        }

        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            var newValue = e.NewTextValue?.Trim() ?? string.Empty;

            if (newValue == _currentValue) return;

            NameChanged?.Invoke(this, (_currentValue, newValue));
            _currentValue = newValue;
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentValue)) return;
            DeleteClicked?.Invoke(this, _currentValue);
        }
    }
}
