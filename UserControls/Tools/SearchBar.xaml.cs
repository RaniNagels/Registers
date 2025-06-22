using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Registers.UserControls.Tools
{
    /// <summary>
    /// Interaction logic for SearchBar.xaml
    /// </summary>
    public partial class SearchBar : System.Windows.Controls.UserControl
    {
        private readonly string _placeholder = "type to search...";
        private bool _isPlaceholderVisible = true;

        public SearchBar()
        {
            InitializeComponent();
            Loaded += (s, e) => ShowPlaceholder();
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(SearchBar),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_isPlaceholderVisible)
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = System.Windows.Media.Brushes.Black;
                _isPlaceholderVisible = false;
            }
            else
            {
                SearchTextBox.SelectAll();
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                ShowPlaceholder();
            }
        }

        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus(); // Remove focus
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isPlaceholderVisible)
            {
                Text = SearchTextBox.Text;
            }
        }

        private void ShowPlaceholder()
        {
            _isPlaceholderVisible = true;
            SearchTextBox.Text = _placeholder;
            SearchTextBox.Foreground = System.Windows.Media.Brushes.LightGray;
        }
    }
}
