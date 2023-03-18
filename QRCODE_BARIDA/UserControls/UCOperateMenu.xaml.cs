using QRCODE_BARIDA.PlcConnectivity;
using QRCODE_BARIDA.PlcWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for UCOperateMenu.xaml
    /// </summary>
    public partial class UCOperateMenu : UserControl
    {
        public bool flashorPlc1 = false;
        Thread thr1;
        public static UCOperateMenu _instance = new UCOperateMenu();
        public static UCOperateMenu Instance
        {
            get
            {
                return _instance;
            }
        }
        private UCOperateMenu()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //CheckForIllegalCrossThreadCalls = false;
            thr1 = new Thread(PLC1Loop.method1);
         //   thr1.Start();
        }
    }
    public static class PLC1Loop
    {
        public static void method1()
        {
            while (true)
            {

                try
                {
                    if (Enum.GetName(typeof(ConnectionState), S7Plc1.Instance.ConnectionState).Equals("online"))
                    {
                        UCOperateMenu.Instance.flashorPlc1 = !UCOperateMenu.Instance.flashorPlc1;
                        if (UCOperateMenu.Instance.flashorPlc1) UCOperateMenu.Instance.plc1State.Background = new SolidColorBrush(Colors.Lime);
                        else UCOperateMenu.Instance.plc1State.Background = new SolidColorBrush(Colors.Green); ;

                    }
                    else if (Enum.GetName(typeof(ConnectionState), S7Plc1.Instance.ConnectionState).Equals("connecting"))
                    {

                    }
                    else
                    {
                        UCOperateMenu.Instance.flashorPlc1 = !UCOperateMenu.Instance.flashorPlc1;
                        if (UCOperateMenu.Instance.flashorPlc1) UCOperateMenu.Instance.plc1State.Background = new SolidColorBrush(Colors.Orange);
                        else UCOperateMenu.Instance.plc1State.Background = new SolidColorBrush(Colors.Red);
                    }
                }
                catch
                {

                }
                Thread.Sleep(200);
            }
        }
    }
}

