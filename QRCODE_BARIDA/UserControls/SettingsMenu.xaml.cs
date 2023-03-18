using AForge.Video.DirectShow;
using QRCODE_BARIDA.Classes;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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


namespace QRCODE_BARIDA.UserControls
{
    /// <summary>
    /// SettingsMenu.xaml etkileşim mantığı
    /// </summary>
    public partial class SettingsMenu : UserControl
    {
        FilterInfoCollection filterInfoCollection;
     //   VideoCaptureDevice videoCaptureDevice;
        public SettingsMenu()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(Enum.GetName(typeof(Roles), TemporaryMemory.Roles).Equals("Admin") || Enum.GetName(typeof(Roles), TemporaryMemory.Roles).Equals("superuser"))
            {
                this.IsEnabled = true;
            }
            else
            {
                this.Visibility = Visibility.Hidden;
                
            }
            comPort.IsEnabled = false;
            ipAddressText.IsEnabled = false;
            portText.IsEnabled = false; 
            webcamDevices.IsEnabled = false;
            baudRate.IsEnabled  = false;
        }
        #region Set Settings

        private void checkWebCam_Checked(object sender, RoutedEventArgs e)
        {
            if (checkWebCam.IsChecked == true)
            {
                checkIpCam.IsChecked = false;
                checkSeriPort.IsChecked = false;
                comPort.IsEnabled = false;
                ipAddressText.IsEnabled = false;
                portText.IsEnabled = false;
                webcamDevices.IsEnabled = true;
                baudRate.IsEnabled = false;
                filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach(FilterInfo filterInfo in filterInfoCollection)
                {
                    webcamDevices.Items.Add(filterInfo.Name);
                }
                webcamDevices.SelectedIndex = 0;
            }
        }

        private void checkIpCam_Checked(object sender, RoutedEventArgs e)
        {
            if (checkIpCam.IsChecked == true)
            {
                checkWebCam.IsChecked = false;
                checkSeriPort.IsChecked = false;
                ipAddressText.IsEnabled = true;
                portText.IsEnabled = true;
                comPort.IsEnabled = false;
                webcamDevices.IsEnabled = false;
                baudRate.IsEnabled = false;
            }

        }

        private void checkSeriPort_Checked(object sender, RoutedEventArgs e)
        {
            if (checkSeriPort.IsChecked == true)
            {

                checkIpCam.IsChecked = false;
                checkWebCam.IsChecked = false;
                comPort.IsEnabled = true;
                ipAddressText.IsEnabled = false;
                portText.IsEnabled = false;
                webcamDevices.IsEnabled = false;
                baudRate.IsEnabled = true;
                string[] portlar = SerialPort.GetPortNames(); // Bağlı seri portları diziye aktardık
                foreach (string portAdi in portlar)
                {
                    comPort.Items.Add(portAdi);
                }
            }
        }

        #endregion

        #region Save Settings
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Enum.GetName(typeof(Roles), TemporaryMemory.Roles).Equals("Admin"))
            {
                return;
            }

                if (checkWebCam.IsChecked == true)
            {
                TemporaryMemory.ScannerType = ScannerType.webcam;
                TemporaryMemory.VideoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[webcamDevices.SelectedIndex].MonikerString);
                

            }
            else if(checkSeriPort.IsChecked == true)
            {
                TemporaryMemory.ScannerType = ScannerType.barcodeScanner;

            }

            else if(checkIpCam.IsChecked == true)
            {
                TemporaryMemory.ScannerType = ScannerType.ipcam;
            }
            else
            {
                TemporaryMemory.ScannerType = ScannerType.none;
                MessageBox.Show("Hiçbir seçenek seçilmedi");
                return;
            }
            MessageBox.Show(
            System.Enum.GetName(typeof(ScannerType), TemporaryMemory.ScannerType));
        }

        #endregion

        private void xxx_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key.ToString());
        }
    }
}
