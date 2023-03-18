using QRCODE_BARIDA.PlcWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QRCODE_BARIDA.PlcConnectivity
{
    public class S7Plc1
    {

        private static readonly Lazy<S7Plc1> _instance = new Lazy<S7Plc1>(() => new S7Plc1());
        public static S7Plc1 Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        IPLC plcDriver;
        //System.Timers.Timer timer = new System.Timers.Timer();
        public ConnectionState ConnectionState { get { return plcDriver != null ? plcDriver.ConnectionState : ConnectionState.offline; } }
        ////private S7Plc1()
        ////{
            
        ////}

        public void connect(string ip)
        {
            plcDriver = new S7PlcDriver(S7.Net.CpuType.S71200, ip, 0, 1);
            plcDriver.connect();
        }
        public void disconnect()
        {
            if (plcDriver == null || this.ConnectionState == ConnectionState.offline)
            {
                return;
            }
            plcDriver.disconnect();
        }
    }
}
