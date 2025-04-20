using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;

namespace ListManagerApp
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper;

        public MainWindow()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            LoadItemsFromDatabase();
        }

        private void LoadItemsFromDatabase()
        {
            try
            {
                ItemsListBox.Items.Clear();
                var items = dbHelper.GetItems();
                foreach (var item in items)
                {
                    ItemsListBox.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading items: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddItemWindow addWindow = new AddItemWindow();
            if (addWindow.ShowDialog() == true)
            {
                dbHelper.AddItem(addWindow.NewItem);
                dbHelper.LogAction("ADD", addWindow.NewItem);
                LoadItemsFromDatabase();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsListBox.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select exactly one item to edit.", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            EditItemWindow editWindow = new EditItemWindow(ItemsListBox.SelectedItem.ToString());
            if (editWindow.ShowDialog() == true)
            {
                ItemsListBox.Items[ItemsListBox.SelectedIndex] = editWindow.EditedItem;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one item to delete.", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {ItemsListBox.SelectedItems.Count} item(s)?",
                                                    "Confirm Delete",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var selectedItems = ItemsListBox.SelectedItems.Cast<string>().ToList();

                // Удаляем из базы данных
                dbHelper.DeleteItems(selectedItems);

                // Логируем действие
                foreach (var item in selectedItems)
                {
                    dbHelper.LogAction("DELETE", item);
                }

                // Удаляем из ListBox
                foreach (var item in selectedItems)
                {
                    ItemsListBox.Items.Remove(item);
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("This will permanently delete all items. Continue?",
                                                  "Warning",
                                                  MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Вариант с транзакцией
                    dbHelper.ClearAllItemsWithTransaction();

                    // Или обычный вариант
                    // dbHelper.ClearAllItems();

                    ItemsListBox.Items.Clear();

                    MessageBox.Show("All items have been removed.", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Clearing failed: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close the window?", "Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}