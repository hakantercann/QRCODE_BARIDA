using QRCODE_BARIDA.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCODE_BARIDA.s
{
    public class QrCodeLoginOp
    {
        public bool isCheck(string qr_text)
        {
            
            SqlConnection connection = Database.GetSqlConnection();
            string sSQL = "Select COUNT(1) FROM users WHERE userID = @qr_text";
            SqlCommand cmd = new SqlCommand(sSQL, connection);
            cmd.Parameters.AddWithValue("@qr_text", qr_text);
            ////cmd.Parameters.Add("@qr_text", SqlDbType.Int);
            ////cmd.Parameters["@qr_text"].Value = qr_text;
            int count = (int)Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            if (count == 1)
            {
                
                return true;
            }
            else
            {
                return false;
            }
        }
        public void qrLogin(string qr_text)
        {
            
            if(true)
            {
                SqlConnection connection = Database.GetSqlConnection();
                string sSQL = "Select * from qrLoginFetch Where qrLoginFetch.UserID = @qr_text";
                SqlCommand cmd = new SqlCommand(sSQL,connection);
                cmd.CommandType = CommandType.Text; 

                cmd.Parameters.AddWithValue("@qr_text", qr_text);
                ////cmd.Parameters.Add("@qr_text", SqlDbType.Int);
                ////cmd.Parameters["@qr_text"].Value = qr_text;

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    string firstname = string.Empty;
                    string lastname = string.Empty;
                    string role = string.Empty;
                    while(rdr.Read())
                    {
                        firstname = rdr["firstname"].ToString();
                        lastname = rdr["lastname"].ToString();
                        LastLogin.UserUniq = rdr["userID"].ToString();
                        role =  rdr["roleName"].ToString();
                    }
                    connection.Close();
                    int temp = (int)Enum.Parse(typeof(Roles), role);
                    TemporaryMemory.Roles = (Roles)temp;
                    LastLogin.isLoginned = true;
                    LastLogin.FullName = firstname + " " + lastname;
                //    LastLogin.UserUniq = Convert.ToInt32(qr_text);
                }

            }
            else
            {
               
            }
        }
    }
}
