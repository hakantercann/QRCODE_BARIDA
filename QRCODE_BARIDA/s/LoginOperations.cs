using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using QRCODE_BARIDA.Classes;
using System.Windows;

namespace QRCODE_BARIDA.s
{
    public class LoginOperations
    {
        public bool Login(string user, string pass)
        {
            SqlConnection connection = Database.GetSqlConnection();
            string sSQL = "SELECT COUNT(1) FROM userLogin WHERE [username] = @username AND [userpass] = @password";
            SqlCommand sqlCommand = new SqlCommand(sSQL, connection);
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Parameters.AddWithValue("@username", user);
            sqlCommand.Parameters.AddWithValue("@password", pass);
            int count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            connection.Close();
            if (count == 1)
            {
                GetRole(user, pass);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void GetInfosForHand(string user, string pass)
        {//view oluşturalacak
            string firstname = string.Empty;
            string lastname = string.Empty;
            string role = string.Empty;

            SqlConnection connection = Database.GetSqlConnection();
            string sSQL = "Select p.UserID, p.FirstName, p.LastName,p.RoleName from passLoginFetch p WHERE  p.UserName = @username " +
                "And p.UserPass = @password";
            SqlCommand sqlCommand = new SqlCommand(sSQL, connection);
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Parameters.AddWithValue("@username", user);
            sqlCommand.Parameters.AddWithValue("@password", pass);
            using (SqlDataReader rdr = sqlCommand.ExecuteReader())
            {
                while (rdr.Read())
                {
                    firstname = rdr["firstname"].ToString();
                    lastname = rdr["lastname"].ToString();
                    LastLogin.UserUniq = rdr["userID"].ToString();
                    role = rdr["roleName"].ToString();
                }
            }
            connection.Close();
           
            int temp = (int)Enum.Parse(typeof(Roles), role);
            TemporaryMemory.Roles = (Roles)temp;
            LastLogin.isLoginned = true;
            LastLogin.FullName = firstname + " " + lastname;
        }

        public void GetRole(string user, string pass)
        {
            string role = string.Empty;
            SqlConnection connection = Database.GetSqlConnection();
            string sSQL = "Select r.RoleName\r\nFrom userLogin ul \r\njoin userRoles ur \r\non ur.UserID = ul.UserID\r\njoin roles r \r\non r.RoleID = ur.RoleID\r\nWhere ul.UserID = \r\n(Select userLogin.UserID\r\nFrom userLogin \r\nWhere [username] = @username AND UserPass = @password)";
            SqlCommand sqlCommand = new SqlCommand(sSQL, connection);
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Parameters.AddWithValue("@username", user);
            sqlCommand.Parameters.AddWithValue("@password", pass);
            using (SqlDataReader rdr = sqlCommand.ExecuteReader())
            {
                while (rdr.Read())
                {
                    role = rdr["RoleName"].ToString();
                }
            }

            connection.Close();

            int temp = (int)Enum.Parse(typeof(Roles), role);
            Console.WriteLine(temp);
            //    MessageBox.Show(temp.ToString());
            TemporaryMemory.Roles = (Roles)temp;
            
        }

    }
}
