using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCODE_BARIDA.s
{
    public static class Database
    {
        public static SqlConnection GetSqlConnection()
        {
            string cn_String = Properties.Settings.Default.DbConn;

            SqlConnection cn_connection = new SqlConnection(cn_String);

            if (cn_connection.State != ConnectionState.Open) cn_connection.Open();

            //</ db oeffnen >



            //< output >

            return cn_connection;
        }

        public static void executeSQL(string SQL_Text)
        {
            //--------< Execute_SQL() >--------
            SqlConnection cn_connection = GetSqlConnection();
            SqlCommand cmd_Command = new SqlCommand(SQL_Text, cn_connection);
            Console.WriteLine(SQL_Text);
            cmd_Command.ExecuteNonQuery();
            cn_connection.Close();

        }
        public static DataTable Get_DataTable(string SQL_Text)
        {
            //--------< db_Get_DataTable() >--------
            SqlConnection cn_connection = GetSqlConnection();
            //< get Table >
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(SQL_Text, cn_connection);
            adapter.Fill(table);
            //</ get Table >
            //< output >
            cn_connection.Close();
            return table;

            //</ output >

            //--------</ db_Get_DataTable() >--------
        }
    }
}
