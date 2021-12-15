using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace AccoutingVolunteers
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string connectionString = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=postgres;Database=db_Volunteers;";
        private NpgsqlDataAdapter adapter;
        private NpgsqlCommandBuilder comandbuilder;
        private NpgsqlConnection connection;
        private DataTable usersTable;

        public MainWindow()
        {
            InitializeComponent();
            Dispatcher.BeginInvoke((Action)(() => FillTable()));
            Dispatcher.BeginInvoke((Action)(() => FillComboBox()));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (usersGrid.SelectedItems != null)
            {
                for (int i = 0; i < usersGrid.SelectedItems.Count; i++)
                {
                    DataRowView datarowView = usersGrid.SelectedItems[i] as DataRowView;
                    if (datarowView != null)
                    {
                        DataRow dataRow = datarowView.Row;
                        dataRow.Delete();
                    }
                }
            }
            Dispatcher.BeginInvoke((Action)(() => UpdateDB()));
        }

        private void FillComboBox()
        {
            List<string> languageCollection = (from DataRow row in usersTable.Rows select row.ItemArray[3].ToString()).ToList();
            languageCollection.Sort();
            comboBoxLanguage.ItemsSource = languageCollection.Distinct();

            List<string> genderCollection = (from DataRow row in usersTable.Rows select row.ItemArray[5].ToString()).ToList();
            genderCollection.Sort();
            comboBoxGender.ItemsSource = genderCollection.Distinct();

            List<string> nationalityCollection = (from DataRow row in usersTable.Rows select row.ItemArray[6].ToString()).ToList();
            nationalityCollection.Sort();
            comboBoxNationality.ItemsSource = nationalityCollection.Distinct();
        }

        private void FillTable()
        {
            try
            {
                connection = new NpgsqlConnection(connectionString);
                var sqlQuery = "SELECT * FROM volunteers;";
                using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                {
                    adapter = new NpgsqlDataAdapter(command);
                    usersTable = new DataTable();

                    connection.Open();
                    adapter.Fill(usersTable);
                    comandbuilder = new NpgsqlCommandBuilder(adapter);
                    usersGrid.ItemsSource = usersTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillTableAfterAdd()
        {
            var sqlQuery = "SELECT * FROM users;";
            using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
            {
                adapter = new NpgsqlDataAdapter(command);
                usersTable.Clear();
                adapter.Fill(usersTable);
                comandbuilder = new NpgsqlCommandBuilder(adapter);
                adapter.InsertCommand = comandbuilder.GetInsertCommand();
                adapter.UpdateCommand = comandbuilder.GetUpdateCommand();
                adapter.DeleteCommand = comandbuilder.GetDeleteCommand();
                usersGrid.ItemsSource = usersTable.DefaultView;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchQuery = $"SELECT * FROM volunteers WHERE english_skill = '{comboBoxLanguage.SelectedItem}' AND age < '{ageTextBox.Text}' AND gender = '{comboBoxGender.SelectedItem}' AND nationality = '{comboBoxNationality.SelectedItem}';";
            using (NpgsqlCommand command = new NpgsqlCommand(searchQuery, connection))
            {
                adapter = new NpgsqlDataAdapter(command);
                usersTable.Clear();
                adapter.Fill(usersTable);
                comandbuilder = new NpgsqlCommandBuilder(adapter);
                usersGrid.ItemsSource = usersTable.DefaultView;
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e) => new ChoiceWindow(usersGrid.SelectedItems).ShowDialog();

        private void UpdateButton_Click(object sender, RoutedEventArgs e) => Dispatcher.BeginInvoke((Action)(() => UpdateDB()));

        private void UpdateDB() => adapter.Update(usersTable);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connection.CloseAsync();
        }
    }
}