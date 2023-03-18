using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCODE_BARIDA.Classes
{
    public static class TemporaryMemory
    {
        public static Roles Roles { get; set; }
        public static ScannerType ScannerType { get; set; }
        public static VideoCaptureDevice VideoCaptureDevice { get; set; }
        public static string InfoTextPublic { get; set; }
    }
}
