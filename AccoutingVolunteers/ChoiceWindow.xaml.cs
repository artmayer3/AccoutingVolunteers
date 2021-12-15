using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Windows;

namespace AccoutingVolunteers
{
    /// <summary>
    /// Логика взаимодействия для ChoiceWindow.xaml
    /// </summary>
    public partial class ChoiceWindow : Window
    {
        public ChoiceWindow(IList usersList)
        {
            InitializeComponent();
            usersGrid.ItemsSource = usersList;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\Согласия"))
            {
                Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\Согласия");
            }

            try
            {
                foreach (DataRowView row in usersGrid.SelectedItems)
                {
                    File.Copy("Patterns\\шаблонРоссия.docx", $"Согласия\\{row.Row.ItemArray[0]} {row.Row.ItemArray[1]} (согласие Россия).docx");

                    if ("Украина".Equals(row.Row.ItemArray[6].ToString()))
                        File.Copy("Patterns\\шаблонУкраина.docx", $"Согласия\\{row.Row.ItemArray[0]} {row.Row.ItemArray[1]} (согласие Украина).docx");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Close();
        }
    }
}