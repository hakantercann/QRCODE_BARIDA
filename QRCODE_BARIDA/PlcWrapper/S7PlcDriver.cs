using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCODE_BARIDA.PlcWrapper
{
    public class S7PlcDriver : IPLC
    {
        Plc client;
        
        public S7PlcDriver(CpuType cpu, string ip, short rack, short slot)
        {
            client = new Plc(cpu, ip, rack, slot);
        }
        private ConnectionState _connectionState;
        public ConnectionState ConnectionState
        {
            get
            {
                return _connectionState;
            }
            private set
            {
                _connectionState = value;
            }
        }

        public void connect()
        {
            try
            {
                ConnectionState = ConnectionState.connecting;
                client.Open();
                if(client.IsConnected)
                {
                    ConnectionState = ConnectionState.online;
                }
                else
                {
                    ConnectionState = ConnectionState.offline;
                    throw new Exception("Cannot Connect");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void disconnect()
        {
            ConnectionState = ConnectionState.offline;
            client.Close();
        }
    }
}
