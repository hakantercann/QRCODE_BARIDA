using AForge.Video;
using AForge.Video.DirectShow;
using QRCODE_BARIDA.Classes;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZXing;
using AForge.Imaging.Filters;
using QRCODE_BARIDA.s;
using System.Data.SqlClient;
using System.Windows.Input;
using QRCODE_BARIDA.PopUps;

namespace QRCODE_BARIDA.UserControls
{
    /// <summary>
    /// MainMenu.xaml etkileşim mantığı
    /// </summary>
    public partial class MainMenu : UserControl
    {
        #region Local variables, instances, control variables
        LogOperate logOperate;
        MJPEGStream videoSource;
        VideoCaptureDevice captureDevice;
        DispatcherTimer dispatcherTimer;
        bool scannerActive = false;
        QrCodeLoginOp qrCodeLoginOp;
        #endregion

        #region Initilize Window and empty timer

        public MainMenu()
        {
            InitializeComponent();
            qrCodeLoginOp = new QrCodeLoginOp();
            logOperate = new LogOperate();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 30);
            NumPad numPad = new NumPad();
            numPad.ShowDialog();

            
        //    dispatcherTimer.Start();
            ////VideoDevices = new ObservableCollection<FilterInfo>();
            ////filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);   
            ////foreach(FilterInfo filterInfo in filterInfoCollection)
            ////{
            ////    VideoDevices.Add(filterInfo);
            ////}
            ////if(VideoDevices.Any())
            ////{
            ////    captureDevice = new VideoCaptureDevice(VideoDevices[0].MonikerString);
            ////    captureDevice.NewFrame += new NewFrameEventHandler(captureDevice_NewFrame);
            //////    captureDevice.Start();
            ////}

        }

        private void dispatcherTimer_tick(object sender, EventArgs e)
        {
   /*         string sSQL = "Select * from logView";
            logTable.ItemsSource = Database.Get_DataTable(sSQL).DefaultView;
            qrCodeDelay = false;*/
            //Console.WriteLine(Tools.barcodeScan(lastFrame));
            ////BarcodeReader qrReader = new BarcodeReader();
            ////Result res = qrReader.Decode();
            ////if (res != null)
            ////{
            ////    qrText.Text = res.ToString();
            ////}
            ////else
            ////{
            ////    qrText.Text = null;
            ////}
        }

        #endregion

        public string QrText { get; set; }
        #region Qr login with using cam

        #region Ipcam
        private void ipcam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            try
            {
                bool isLastScanned = false;
                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {

                    Mirror filter = new Mirror(false, true);
                    // apply the filter
                    filter.ApplyInPlace(bitmap);
                    bi = ToBitmapImage(bitmap);
                    ///Eklenebilir
                    BarcodeReader qrReader = new BarcodeReader();
                    Result res = qrReader.Decode(bitmap);
                    if (res != null)
                    {
                        Console.WriteLine(res.ToString());
                        QrText = res.ToString();
                        isLastScanned = true;
                        string sql = "Insert Into logs(UserID, qrID) VALUES ( " + 1 + ", \r\n(Select qrID FROM qrcodes WHERE qrCode = '" + QrText + "'))";
                        Database.executeSQL(sql);//null dönüş olabilir hata çıktısı gelecek ayrı class yapılacak 
                        //null daki hatalar hat loguna düşecek
                        //son ekran görüntüsü sol alttaki image kısmına gelecek
                        //sol üstte datatable da işlemde operatörün yaptıkları gözükecek
                        Thread.Sleep(200);
                    }

                }
                bi.Freeze(); // avoid cross thread operations and prevents leaks
                Dispatcher.BeginInvoke(new ThreadStart(delegate { frameHolder.Source = bi; }));
                if (isLastScanned == true)
                {
                 //   Dispatcher.BeginInvoke(new ThreadStart(delegate { lastScanned.Source = bi; }));
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _webSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCameraWeb();
            }
        }
        private void StopCameraIp()
        {
            videoSource.Stop();
        }
        #endregion

        #region WebCam
        private void captureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                bool isLastScanned = false;
                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {

                    Mirror filter = new Mirror(false, true);
                    // apply the filter
                    filter.ApplyInPlace(bitmap);
                    bi = ToBitmapImage(bitmap);
                    ///Eklenebilir
                    BarcodeReader qrReader = new BarcodeReader();
                    Result res = qrReader.Decode(bitmap);
                    if (res != null)
                    {
                        //////
                        ///...
                        //////
                        isLastScanned = sqlOperations(res, bi);
                    }


                }
                bi.Freeze(); // avoid cross thread operations and prevents leaks
                Dispatcher.BeginInvoke(new ThreadStart(delegate { frameHolder.Source = bi; }));
                if (isLastScanned == true)
                { 
                  //  Dispatcher.BeginInvoke(new ThreadStart(delegate { lastScanned.Source = bi; })); 
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _webSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCameraWeb();
            }
        }
        private void StopCameraWeb()
        {
            if (captureDevice != null)
            {
                captureDevice.SignalToStop();
                captureDevice.WaitForStop();
                captureDevice.NewFrame -= new NewFrameEventHandler(captureDevice_NewFrame);
            }
            captureDevice = null;
        }

        #endregion
        private bool sqlOperations(Result res, BitmapImage bi)
        {
            if (!LastLogin.isLoginned)
            //kullanıcı kontrolü
            {
                Console.WriteLine(res.ToString());
                QrText = res.ToString();
                try
                {
                    if (qrCodeLoginOp.isCheck(QrText))
                    {
                        qrCodeLoginOp.qrLogin(QrText);
                        
                        if (!SessionWithThread.isStart)
                        {
                            SessionWithThread.startSession();
                        }
                        Console.WriteLine(LastLogin.FullName);
                        Console.WriteLine(LastLogin.UserUniq);
                        Console.WriteLine(TemporaryMemory.Roles);
                        logOperate.logSave(LastLogin.UserUniq);
                        //images
                        JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                        jpg.Frames.Add(BitmapFrame.Create(bi));
                        using (Stream stm = File.Create("C:\\Users\\hakan\\source\\repos\\QRCODE_BARIDA\\QRCODE_BARIDA\\logimages\\im.jpg"))
                        {
                            jpg.Save(stm);

                        }
                        //logOperate.logImageSave("C:\\Users\\hakan\\source\\repos\\QRCODE_BARIDA\\QRCODE_BARIDA\\logimages\\im.jpg");
                        return true;
                        //                                    logOperate.logImageSave("C:\\Users\\hakan\\source\\repos\\QRCODE_BARIDA\\QRCODE_BARIDA\\logimages\\im.jpg");
                    }
                }
                catch (SqlException ex)
                {
                    Tools.SqlExCatcher(ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                Thread.Sleep(100);

            }
            return false;
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }
        #endregion

        #region QrLogin with using barcode scanner 
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
      /*      if (!scannerActive)
            {
                return;
            }
            Console.WriteLine(e.Key);
            if (e.Key == Key.Return)
            {
                QrText = qrText.Text;
                qrText.Text = "";
                Console.WriteLine(QrText);
                sqlOperations(QrText);
            }
      */
        }
        private void qrText_KeyDown(object sender, KeyEventArgs e)
        {
            if (!scannerActive)
            {
                return;
            }
            Console.WriteLine(e.Key);
            if (e.Key == Key.Return)
            {
                QrText = qrText.Text;
                qrText.Text = "";
                Console.WriteLine(QrText);
                if(sqlOperations(QrText))
                {
                    qrText.Text = "";
                    qrText.IsEnabled = false ;
                }
                else
                {
                    qrText.Text = "";
                }
            }

        }


        private bool sqlOperations(string res)
        {
            if (!LastLogin.isLoginned)
            //kullanıcı kontrolü
            {

                try
                {
                    if (qrCodeLoginOp.isCheck(QrText))
                    {
                        qrCodeLoginOp.qrLogin(QrText);
                        if (!SessionWithThread.isStart)
                        {
                            SessionWithThread.startSession();
                        }
                        Console.WriteLine(LastLogin.FullName);
                        Console.WriteLine(LastLogin.UserUniq);
                        Console.WriteLine(TemporaryMemory.Roles);
                        logOperate.logSave(LastLogin.UserUniq);
                        return true;
                        //images

                        //logOperate.logImageSave("C:\\Users\\hakan\\source\\repos\\QRCODE_BARIDA\\QRCODE_BARIDA\\logimages\\im.jpg");

                        //                                    logOperate.logImageSave("C:\\Users\\hakan\\source\\repos\\QRCODE_BARIDA\\QRCODE_BARIDA\\logimages\\im.jpg");
                    }
                }
                catch (SqlException ex)
                {
                    Tools.SqlExCatcher(ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                Thread.Sleep(100);

            }
            return false;
        }

        #endregion

        #region Stop, Start, Next Button Control according to Scannertype



        private void startScan_Click(object sender, RoutedEventArgs e)
        {
            switch (TemporaryMemory.ScannerType)
            {
                case ScannerType.none:
                    MessageBox.Show("Lütfen ayarları tamamlayınız");
                    break;
                case ScannerType.webcam:
                    try
                    {
                        captureDevice = TemporaryMemory.VideoCaptureDevice;
                        captureDevice.NewFrame += new NewFrameEventHandler(captureDevice_NewFrame);
                        captureDevice.Start();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.StackTrace);
                    }
                    break;
                case ScannerType.ipcam:
                    videoSource = new MJPEGStream("");
                    videoSource.NewFrame += new NewFrameEventHandler(ipcam_NewFrame);
                    videoSource.Start();
                    break;
                case ScannerType.barcodeScanner:
                    qrText.IsEnabled = true;
                    qrText.Focus();
                    qrText.Select(0, qrText.Text.Length);
                    scannerActive = true;
                    qrText.Text = "";
                    break;
            }
        }


        private void stopScan_Click(object sender, RoutedEventArgs e)
        {
            string sSQL = "Select * from logView";//class olacak 
            //yeri burası değill
            logTable.ItemsSource = Database.Get_DataTable(sSQL).DefaultView;
            switch (TemporaryMemory.ScannerType)
            {
                case ScannerType.none:
                    MessageBox.Show("Lütfen ayarları tamamlayınız");
                    break;
                case ScannerType.webcam:
                    StopCameraWeb();
                    break;
                case ScannerType.ipcam:
                    StopCameraIp();
                    break;
                case ScannerType.barcodeScanner:
                    scannerActive = false;
                    break;
            }


        }

        private void nextScan_Click(object sender, RoutedEventArgs e)
        {
            switch (TemporaryMemory.ScannerType)
            {
                case ScannerType.none:
                    MessageBox.Show("Lütfen ayarları tamamlayınız");
                    break;
                case ScannerType.webcam:
                    captureDevice = TemporaryMemory.VideoCaptureDevice;
                    if (captureDevice != null)
                    {
                        frameHolder.Source = null;
                        captureDevice.NewFrame += new NewFrameEventHandler(captureDevice_NewFrame);
                        captureDevice.Start();
                    }
                    break;
                case ScannerType.ipcam:
                    StopCameraIp();
                    break;
                case ScannerType.barcodeScanner:
                    scannerActive = true;
                    break;
            }
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Enum.GetName(typeof(Roles), TemporaryMemory.Roles).Equals("Admin") || Enum.GetName(typeof(Roles), TemporaryMemory.Roles).Equals("superuser"))
            {
                this.IsEnabled = true;
            }
            else
            {
                this.IsEnabled = false;
                this.Visibility = Visibility.Hidden;

            }
        }


    }
}