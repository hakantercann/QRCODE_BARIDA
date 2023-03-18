using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace QRCODE_BARIDA.Classes
{
    public static class SessionWithThread
    {
        public static int i { get; set; }
        private static Thread thread;
        private static int timeout;
        public static bool isStart = false;
        public static void startSession()
        {
            if(isStart)
            {
                return;
            }
            switch (TemporaryMemory.Roles)
            {
                case Roles.none:
                    timeout = 0;
                    break;
                case Roles.Operator:
                    timeout = 50000;
                    break;
                case Roles.Admin:
                    timeout = 1000;
                    break;
                case Roles.superuser:
                    timeout = 5000;
                    break;
            }
            thread = new Thread(SessionCallBack);
            thread.Start();
            i = 0;

            isStart = true;
        }

        private static void SessionCallBack()
        {
            while (true)
            {
                i++;
                Console.WriteLine(i);
                Thread.Sleep(100);
                if (i > timeout)
                {
                    Console.WriteLine("Oturum Sonlandırıldı");

                    stopSession();
                }
            }
        }
        public static void stopSession()
        {
            //emporaryMemory.VideoCaptureDevice = null;
            if(!LastLogin.isLoginned)
            {
                return;
            }
            TemporaryMemory.Roles = Roles.none;
            LastLogin.isLoginned = false;
            LastLogin.FullName = string.Empty;
            LastLogin.UserUniq = string.Empty;
            i = 0;
            
            isStart = false;
            
            thread.Abort();
        }
    }
}
