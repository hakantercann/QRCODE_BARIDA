using QRCODE_BARIDA.Classes;
using QRCODE_BARIDA.Models;
using QRCODE_BARIDA.s;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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

namespace QRCODE_BARIDA.UserControls
{
    /// <summary>
    /// Interaction logic for LogMenu1.xaml
    /// </summary>
    public partial class LogMenu1 : UserControl
    {
        Log logModel;
        List<Log> listModel;
        public LogMenu1()
        {
            InitializeComponent();
            
            listModel = new List<Log>();
        }


        private void fetchLogView()
        {
            SqlConnection conn = Database.GetSqlConnection();
            string sSQL = "Select firstname, lastname, productionDate, LogID  From logView ORDER BY productionDate DESC";
            SqlCommand cmd= new SqlCommand(sSQL, conn); 
            cmd.CommandType = CommandType.Text;
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    logModel = new Log();
                    logModel.FirstName = reader["firstName"].ToString();
                    logModel.LastName = reader["lastname"].ToString();
                    logModel.DateTime = reader["productionDate"].ToString();
                    logModel.LogID = reader["LogID"].ToString();
                    listModel.Add(logModel);
                }
            }

            conn.Close();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            fetchLogView();
            if(Enum.GetName(typeof(Roles), TemporaryMemory.Roles).Equals("Admin"))
            {
                this.IsEnabled = true;
                this.Visibility = Visibility.Visible;
                LogDataGrid.ItemsSource = ToDataTable<Log>(listModel).DefaultView;

//                LogDataGrid.ItemsSource = Database.Get_DataTable("Select firstname, lastname, productionDate, LogID  From logView").DefaultView;
            }
            else
            {
                this.IsEnabled = false;
                this.Visibility = Visibility.Hidden;
            }
        }

        private void LogDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Console.WriteLine(LogDataGrid.SelectedIndex);
        }
            public DataTable ToDataTable<T>(List<T> items)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    ////if (prop.Name.Equals("LogID"))
                    ////{
                    ////    continue;
                    ////}//Setting column names as Property names
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                //put a breakpoint here and check datatable
                return dataTable;
            }
        
    }
}
