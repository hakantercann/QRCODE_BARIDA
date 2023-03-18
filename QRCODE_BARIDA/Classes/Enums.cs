using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCODE_BARIDA.Classes
{
    public enum ScannerType
    {
        none,
        webcam,
        ipcam,
        barcodeScanner,
    }
    public enum Roles
    {
        none = 0,
        Operator = 1,
        Admin = 2,
        superuser = 3
    }
}
