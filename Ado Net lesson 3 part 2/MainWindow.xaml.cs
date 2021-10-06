using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
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

namespace Ado_Net_lesson_3_part_2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		SqlConnection conn;
		public MainWindow()
		{
			InitializeComponent();
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			using (conn = new SqlConnection())
			{
				conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
				await conn.OpenAsync();
				SqlCommand command = conn.CreateCommand();
				command.CommandText = "WAITFOR DELAY '00:00:05';";
				command.CommandText += txtbox.Text;

				DataTable table = new DataTable();
				using (var reader = await command.ExecuteReaderAsync())
				{
					do
					{
						bool hasAddedColumn = false;
						while (await reader.ReadAsync())
						{
							if (!hasAddedColumn)
							{
								hasAddedColumn = true;
								for (int i = 0; i < reader.FieldCount; i++)
								{
									table.Columns.Add(reader.GetName(i));
								}
								
							}
							DataRow row = table.NewRow();
							for (int i = 0; i < reader.FieldCount; i++)
							{
								row[i] = await reader.GetFieldValueAsync<object>(i);
							}
							table.Rows.Add(row);
						}
					} while (reader.NextResult());
					MyDataGrid.ItemsSource = table.DefaultView;
				}
			}
		}
	}
}