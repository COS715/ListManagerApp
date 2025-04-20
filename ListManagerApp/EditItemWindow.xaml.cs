using System.Windows;

namespace ListManagerApp
{
    public partial class EditItemWindow : Window
    {
        public string EditedItem { get; private set; }

        public EditItemWindow(string currentItem)
        {
            InitializeComponent();
            ItemTextBox.Text = currentItem;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ItemTextBox.Text))
            {
                EditedItem = ItemTextBox.Text;
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