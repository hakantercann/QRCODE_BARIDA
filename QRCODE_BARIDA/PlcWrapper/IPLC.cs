using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCODE_BARIDA.PlcWrapper
{
    public interface IPLC
    {
        ConnectionState ConnectionState { get; }
        void connect();
        void disconnect();
    }
}
