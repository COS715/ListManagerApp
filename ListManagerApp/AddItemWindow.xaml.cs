using System.Windows;

namespace ListManagerApp
{
    public partial class AddItemWindow : Window
    {
        public string NewItem { get; private set; }

        public AddItemWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ItemTextBox.Text))
            {
                NewItem = ItemTextBox.Text;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Item cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Добавляем этот метод для обработки кнопки Cancel
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}