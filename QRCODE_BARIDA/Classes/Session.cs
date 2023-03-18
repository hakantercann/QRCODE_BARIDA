using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace QRCODE_BARIDA.Classes
{
    public static class Session
    {
        private static Timer _timer = null;
        public static int i { get; set; }
        private static int timeout;
        public static bool isStart = false;
        public static void startSession()
        {
            switch(TemporaryMemory.Roles)
            {
                case Roles.none:
                    timeout = 0;
                    break;
                case Roles.Operator:
                    timeout = 50000;
                    break;
                case Roles.Admin:
                    timeout = 5000;
                    break;
            }
            _timer = new Timer(TimerCallback, null, 0, 100);
            i = 0;
        }

        private static void TimerCallback(object state)
        {
            isStart = true;
            i += 1;
            Console.WriteLine(i);
            if (i > timeout)
            {
                Console.WriteLine("Oturum Sonlandırıldı");
                stopSession();
            }
        }

        public static void stopSession()
        {
           //emporaryMemory.VideoCaptureDevice = null;
            TemporaryMemory.Roles = Roles.none;
            LastLogin.isLoginned = false;
            LastLogin.FullName = string.Empty;
            LastLogin.UserUniq = string.Empty;
            i = 0;
            _timer.Dispose();
            isStart = false;
        }
    }
}
