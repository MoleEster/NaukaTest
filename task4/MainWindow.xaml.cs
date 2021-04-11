using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace task4
{
    public partial class MainWindow : Window
    {
        private static SqlConnection connection;
        private static SqlDataAdapter adapter;
        private static SqlDataReader reader;


        public MainWindow()
        {
            InitializeComponent();
            adapter = new SqlDataAdapter();
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "Nauka",
                Pooling = !true
            };
            connection = new SqlConnection(connectionStringBuilder.ConnectionString);
        }

        private void btnMenuAdd_Click(object sender, RoutedEventArgs e)
        {
            grdAdding.Visibility = Visibility.Visible;
            grdDatabase.Visibility = Visibility.Collapsed;
        }

        private void btnMenuDatabase_Click(object sender, RoutedEventArgs e)
        {
            grdAdding.Visibility =Visibility.Collapsed; 
            grdDatabase.Visibility = Visibility.Visible;
            connection.Open();
            SqlCommand command = new SqlCommand(@"select Document_Number, Order_date,Delivery_contract,Contractor,Storage,Stock_number,Amount,Price,Cost from [dbo].[Order2]", connection);

            command.ExecuteNonQuery();
            DataTable dt = new DataTable();
            using (adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(dt);
                Db.ItemsSource = dt.DefaultView;
            }
            connection.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlCommand command = new SqlCommand($@"SET IDENTITY_INSERT Order2 ON; INSERT INTO [dbo].[Order2](Document_Number, Order_date,Delivery_contract,Contractor,Storage,Stock_number,Amount,Price,Cost)"
                                                    + $" VALUES( {int.Parse(tbDocNum.Text)},'{tbOrdrDt.Text}',{int.Parse(tbDelCon.Text)},'{tbContr.Text}','{tbStorage.Text}',{int.Parse(tbStkNum.Text)},{int.Parse(tbAmount.Text)},{int.Parse(tbPrice.Text)},{int.Parse(tbCost.Text)}) ", connection);
                adapter.InsertCommand = command;
                connection.Open();
                reader = command.ExecuteReader();
                reader.Close();
                MessageBox.Show("Ордер добавлен");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if(Db.SelectedItem != null)
            {
                //int selectedColumn = Db.CurrentCell.Column.DisplayIndex;
                var selectedCell = Db.SelectedCells[0];
                var cellContent = selectedCell.Column.GetCellContent(selectedCell.Item);
                var Result = MessageBox.Show($"Вы уверены что хотите удалить ордер номер {(cellContent as TextBlock).Text}?","Удаление ордера",MessageBoxButton.YesNo);
                if (Result.ToString() == "Yes")
                {
                    try
                    {
                        SqlCommand command = new SqlCommand($@"Delete FROM [dbo].[Order2] WHERE Document_Number={(cellContent as TextBlock).Text} ", connection);
                        adapter.InsertCommand = command;
                        connection.Open();
                        reader = command.ExecuteReader();
                        reader.Close();
                        MessageBox.Show("Ордер удален");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        if (connection != null)
                            connection.Close();
                    }
                }
            }
            btnMenuDatabase_Click(sender,e);
        }

        private void Db_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int selectedColumn = Db.CurrentCell.Column.DisplayIndex;
            var selectedCell = Db.SelectedCells[selectedColumn];
            var cellContent =selectedCell.Column.GetCellContent(selectedCell.Item);
            var selectedCell2 = Db.SelectedCells[0];
            var cellContent2 = selectedCell2.Column.GetCellContent(selectedCell2.Item);
            if (selectedColumn != 0)
            {
                var Result = MessageBox.Show($"Вы уверены что хотите изменить ордер номер {(cellContent2 as TextBlock).Text}?", "Изменение ячейки", MessageBoxButton.YesNo);
                if (Result.ToString() == "Yes")
                {
                    try
                    {
                        SqlCommand command;
                        if (cellContent.GetType() == typeof(string))
                        {
                            command = new SqlCommand($@"UPDATE [dbo].[Order2] SET {Db.Columns[selectedColumn].Header.ToString()} = '{(cellContent as TextBox).Text}' WHERE Document_Number ={(cellContent2 as TextBlock).Text}; ", connection);
                        }
                        else
                        {
                            command = new SqlCommand($@"UPDATE [dbo].[Order2] SET {Db.Columns[selectedColumn].Header.ToString()} = {int.Parse((cellContent as TextBox).Text)} WHERE Document_Number ={(cellContent2 as TextBlock).Text}; ", connection);
                        }
                        adapter.InsertCommand = command;
                        connection.Open();
                        reader = command.ExecuteReader();
                        reader.Close();
                        MessageBox.Show("Ордер изменен");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        if (connection != null)
                            connection.Close();
                    }
                }
                
            }
            else { 
                MessageBox.Show("Нельзя изменять номер документа");
            }
        }

    }
}
