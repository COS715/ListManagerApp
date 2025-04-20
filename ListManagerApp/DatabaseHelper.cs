using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ListManagerApp
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ListDatabaseConnectionString"].ConnectionString;
        }

        public List<string> GetItems()
        {
            List<string> items = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Value FROM Items";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(reader["Value"].ToString());
                }
            }

            return items;
        }

        public void AddItem(string value)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Items (Value) VALUES (@Value)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Value", value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void LogAction(string action, string itemValue, int? itemId = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO History (Action, ItemId, ItemValue) VALUES (@Action, @ItemId, @ItemValue)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Action", action);
                command.Parameters.AddWithValue("@ItemValue", itemValue);

                if (itemId.HasValue)
                    command.Parameters.AddWithValue("@ItemId", itemId);
                else
                    command.Parameters.AddWithValue("@ItemId", DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void DeleteItem(string value)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Items WHERE Value = @Value";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Value", value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteItems(List<string> values)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var value in values)
                {
                    string query = "DELETE FROM Items WHERE Value = @Value";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Value", value);
                    command.ExecuteNonQuery();
                }
            }
        }
        public void ClearAllItemsWithTransaction()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = transaction;

                    // Очищаем основную таблицу
                    command.CommandText = "DELETE FROM Items";
                    command.ExecuteNonQuery();

                    // Очищаем историю (если нужно)
                    command.CommandText = "DELETE FROM History";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}