using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZXing;

namespace QRCODE_BARIDA.Classes
{
    public static class Tools
    {

        public static string SqlExCatcher(SqlException ex)
        {
            string str;
            str = "Source:" + ex.Source;
            str += "\n" + "Message:" + ex.Message;
            str += "\n" + "Inner::" + ex.InnerException;
            str += "\n" + "Line: " + ex.LineNumber;
            str += "\n" + "" + ex.StackTrace;
            str += "\n" + "help link: " + ex.HelpLink;
            return str;
        }
        public static string barcodeScan(Bitmap image)
        {
            try
            { 

                BarcodeReader qrReader = new BarcodeReader();
                Result res = qrReader.Decode((Bitmap) image);
                if (res != null)
                {
                    return res.ToString();
                }
                return null;
            }
            catch(Exception ex)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(ex.Source);
                stringBuilder.Append('\n');
                stringBuilder.Append(ex.Message);
                stringBuilder.Append("\nMessage: ");
                stringBuilder.Append("\nInnerexc: ");
                stringBuilder.Append(ex.InnerException);
                MessageBox.Show(stringBuilder.ToString());
                return "Hata";
            }
        }
    public static void UC_Controller(Grid grid, UserControl userControl)
        {
            
            if(grid.Children.Count > 0)
            {
                if (grid.Children.Contains(userControl))
                {
                    return;
                }
                grid.Children.Clear();
            }
            grid.Children.Add(userControl);
        }
        public static void UC_Controller_Clear(Grid grid)
        {
            grid.Children.Clear();
        }
    }
}
