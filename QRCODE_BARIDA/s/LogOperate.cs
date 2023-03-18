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
    public class LogOperate
    {
        public void logSave(string userID)
        {
            string sql = "Insert Into logs(UserID, productionDate) VALUES ( '" + userID + "', " +
            "'" + DateTime.Now.Year + "- " + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "')";
            Database.executeSQL(sql);
            
            //null dönüş olabilir hata çıktısı gelecek ayrı class yapılacak 
            //null daki hatalar hat loguna düşecek
 
        }
        public void logImageSave(string path)
        {//must declare the scalar variable @ImageData ERRORRRR!!!!!!!!!!!!!!!!!!!!!!!!
            ////SqlConnection conn = Database.GetSqlConnection();
            ////string sSQL = "Insert Into logImages(LogID, logImage) VALUES(" + 1 + ", @ImageDate)";
            ////SqlCommand cmd = new SqlCommand(sSQL, conn);
            ////byte[] imageData = ImageData.pathToImage(path);

            ////cmd.Parameters.Add("@ImageData", SqlDbType.Image);
            ////cmd.Parameters["@ImageData"].Value = imageData;
            ////cmd.ExecuteNonQuery();

            ////conn.Close();
        }
    }
}
