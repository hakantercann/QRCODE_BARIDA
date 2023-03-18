using QRCODE_BARIDA.Classes;
using System;
using System.Windows;
using System.Windows.Input;
using QRCODE_BARIDA.s;
using System.Threading;
using System.Windows.Threading;
using QRCODE_BARIDA.UserControls;
using AForge.Video.DirectShow;
using AForge.Video;
using AForge.Imaging.Filters;
using System.Drawing;
using System.Windows.Media.Imaging;
using ZXing;
using System.Data.SqlClient;
using System.IO;
using System.Drawing.Imaging;
using QRCODE_BARIDA.PlcConnectivity;

namespace QRCODE_BARIDA
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Local variables, instances, control variables
        ////FilterInfoCollection filterInfoCollection;
        ////VideoCaptureDevice videoCaptureDevice;
        Thread mainThread;
        Thread thr1;
        LoginOperations loginOperations;
        BeginSetup setup;
        DispatcherTimer _timer;
        LogOperate logOperate;
        VideoCaptureDevice captureDevice;
        bool scannerActive = false;
        public string QrText { get; set; }
        QrCodeLoginOp qrCodeLoginOp;

        //    Thread sessionInfo;
        #endregion

        #region Initilize Window
        public MainWindow()
        {
            InitializeComponent();
            mainThread = new Thread(mainLoop);
            
            qrCodeLoginOp = new QrCodeLoginOp();
            logOperate = new LogOperate();
            setup = new BeginSetup();
            Console.WriteLine(Enum.GetName(typeof(ScannerType) ,TemporaryMemory.ScannerType));
            
            myPopup.IsOpen = false; ;
            //sessionInfo = new Thread(sessionInfoFunc);
            loginOperations = new LoginOperations();
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(dispatcherTimer_tick);
            _timer.Interval = new TimeSpan(0, 0, 2);
            _timer.Start();
            //sessionInfo.Start();
        }

        private void mainLoop()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Timer
        /// <summary>
        /// Check user instance role and system diagnostic via timer 
        /// </summary>
        private void dispatcherTimer_tick(object sender, EventArgs e)
        {
            if (TemporaryMemory.VideoCaptureDevice == null)
            {
                infoText.Background = System.Windows.Media.Brushes.Red;
                infoText.Text = "Kamera ayarlanmamış";
                
            }
            else
            {
                infoText.Background = System.Windows.Media.Brushes.Green;
                infoText.Text = "";
            }
            switch (TemporaryMemory.Roles)
            {
                case Roles.Admin:
                    infoText.Text = "Admin " + LastLogin.FullName;

                    break;
                case Roles.Operator:
                    infoText.Text = "Operator";
                    infoText.Text += "\n" + LastLogin.FullName;
                    mainThread.Start();
                    thr1.Start();
                    break;
                case Roles.superuser:
                    infoText.Text = "SuperUser";
                    infoText.Text += "\n" + LastLogin.FullName;
                    break;
                case Roles.none:
                    infoText.Text = "Giriş yapılmamış";
                    break;
            }
        }

        #endregion
        #region Rest of All Not big deal 

        private void sessionInfoFunc()
        {
            if (TemporaryMemory.VideoCaptureDevice == null)
            {
                infoText.Background = System.Windows.Media.Brushes.Red;
                //   infoText.Text = "Kamera ayarlanmamış";
            }
            ////else
            ////{
            ////    infoText.Background = System.Windows.Media.Brushes.Green;
            ////    infoText.Text = "";
            ////}
            switch (TemporaryMemory.Roles)
            {
                case Roles.Admin:

                    break;
                case Roles.Operator:
                    break;
                case Roles.none:
                    break;
            }
            Thread.Sleep(1000);
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            LastLogin.isLoginned = false;
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.StackTrace);
                    }
                    break;
                ////case ScannerType.ipcam:
                ////    videoSource = new MJPEGStream("");
                ////    videoSource.NewFrame += new NewFrameEventHandler(ipcam_NewFrame);
                ////    videoSource.Start();
                ////    break;
                ////case ScannerType.barcodeScanner:
                ////    qrText.IsEnabled = true;
                ////    qrText.Focus();
                ////    qrText.Select(0, qrText.Text.Length);
                ////    scannerActive = true;
                ////    qrText.Text = "";
                ////    break;
            }
            //   Tools.UC_Controller(uc_panel, new UserControls.MainMenu());
            /*   TemporaryMemory.ScannerType = ScannerType.webcam;
               TemporaryMemory.Roles = Roles.none;
           */
        }

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
                    if (res != null && !LastLogin.isLoginned)
                    {
                        //////
                        ///...
                        //////
                        isLastScanned = sqlOperations(res, bi);
                    }


                }
                bi.Freeze(); // avoid cross thread operations and prevents leaks
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

        private BitmapImage ToBitmapImage(Bitmap bitmap)
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

        #endregion
        #region Drawer Menu Button Actions

        #region QrLogin Menu
        /// <summary>
        ///  User Qr Login menu 
        /// </summary>
        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
         //   Tools.UC_Controller(uc_panel, new UserControls.SettingsMenu());
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        { 
       //     Tools.UC_Controller(uc_panel, new UserControls.SettingsMenu());
        }

        private void ListViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Enum.GetName(typeof(Roles), TemporaryMemory.Roles).Equals("superuser"))
            {
                return;
            }
            else
            {
                Tools.UC_Controller(uc_panel, new UserControls.SettingsMenu());
            }
        }
        #endregion


        #region Setting Menu Transfer 

        /// <summary>
        /// Qr scan method changing menu
        /// </summary>
        /// 
        private void settingsMenuTransfer()
        {
            if (!Enum.GetName(typeof(Roles), TemporaryMemory.Roles).Equals("superuser"))
            {
                return;
            }
            else
            {
                Tools.UC_Controller(uc_panel, new UserControls.MainMenu());
            }
        }
        private void ListViewItem_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            settingsMenuTransfer();
        }

        private void Image_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
      //      settingsMenuTransfer();
        }

        private void TextBlock_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
        //    settingsMenuTransfer();
        }
        #endregion
        #region Manuel login Operation
        /// <summary>
        /// Login Popup Menu Button Click
        /// </summary>
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            ////QrCodeLoginOp qrCodeLoginOp = new QrCodeLoginOp();
            ////try
            ////{

            ////    Console.WriteLine(qrCodeLoginOp.isCheck(1101101101));
            ////    qrCodeLoginOp.qrLogin(1101101101);
            ////}

            ////catch(SqlException ex){ Console.WriteLine(ex.Message); }      
            if(SessionWithThread.isStart)
            {
                return;
            }

            if (loginOperations.Login(UserBox.Text, passWordBox.Password.ToString()))
            {
                LogOperate logOperate = new LogOperate();
                loginOperations.GetInfosForHand(UserBox.Text, passWordBox.Password);
                SessionWithThread.startSession();
                //new Thread(sessionStart).Start();
                myPopup.IsOpen = false;
                qrLogPop.IsOpen = false;
                LastLogin.isLoginned = true;
                
                Tools.UC_Controller_Clear(uc_panel);
                logOperate.logSave(LastLogin.UserUniq);
                
            }
            else
            {
                MessageBox.Show("Hatalı Giriş");
            }
            
            
        }
        #endregion


        #region Login Popup Menu Transfer
        /// <summary>
        /// Login Popup Menu where on Drawer Menu
        /// </summary>

        private void loginPopUpTransferFunc()
        {
            //////if(myPopup.IsOpen == false && qrLogPop.IsOpen == false && LastLogin.isLoginned == false)
            //////{
            //////    myPopup.IsOpen = true;
            //////    popUpControl = false;

            //////    return;
            //////}
            //////if (!LastLogin.isLoginned)
            //////{
            //////    myPopup.IsOpen = popUpControl;
            //////    popUpControl = popUpControl ? false : true;
            //////    qrLogPop.IsOpen = popUpControl ? true : false;
            //////}
            if (!LastLogin.isLoginned)
            {
                if (myPopup.IsOpen == true)
                {
                    myPopup.IsOpen = false;
                }
                else
                {
                    myPopup.IsOpen = true;
                }
            }
        }
        
        private void ListViewItem_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            loginPopUpTransferFunc();
        }

        private void Image_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
    /*        if (myPopup.IsOpen == true)
            {
                myPopup.IsOpen = false;
            }
            else
            {
                myPopup.IsOpen = true;
            }
      */  }
        private void TextBlock_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
        /*    if (myPopup.IsOpen == true)
            {
                myPopup.IsOpen = false;
            }
            else
            {
                myPopup.IsOpen = true;
            }
          */  //loginPopUpTransferFunc();
        }

        #endregion
        #region Sign out Transfer
        /// <summary>
        /// Sign Out
        /// </summary>
        private void signOutFunc()
        {
            SessionWithThread.stopSession();
        }
        private void Image_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
     //       signOutFunc();
        }

        private void TextBlock_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
       //     signOutFunc();
        }

        private void ListViewItem_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
            signOutFunc();
        }

        #endregion
        #region İş Menü Transferi
        private void operateMenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Tools.UC_Controller(uc_panel, UCOperateMenu.Instance);
        }

        #endregion
        #region Log Menu Tranfer
        /// <summary>
        /// /LogMenu Geçiş
        /// </summary>
        private void logMenuTransfer()
        {
            if (Enum.GetName(typeof(Roles), TemporaryMemory.Roles).Equals("Admin"))
            {
                Tools.UC_Controller(uc_panel, new LogMenu1());
            }
        }
        private void LogMenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            logMenuTransfer();
        }

        private void Image_MouseUp_4(object sender, MouseButtonEventArgs e)
        {
      //      logMenuTransfer();
        }

        private void TextBlock_MouseUp_4(object sender, MouseButtonEventArgs e)
        {
       //     logMenuTransfer();
        }
        #endregion

        #endregion


        
    }

    /*    private void sessionStart()
        {
            Console.WriteLine("sssss");
            if(Session.i > 1000)
            {
                MessageBox.Show("Oturum Sonlandırıldı");
                TemporaryMemory.Roles = Roles.none;
                TemporaryMemory.ScannerType = ScannerType.none;
                TemporaryMemory.VideoCaptureDevice = null;
            }
        }*/

}
