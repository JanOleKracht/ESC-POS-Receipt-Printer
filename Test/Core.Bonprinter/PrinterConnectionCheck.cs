using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

using System.Net.Sockets;

namespace Test
{
    public static class PrinterConnectionCheck
    {
        public static bool PrinterReachable(string ip, int port)
        {
            try
            {
                using var client = new TcpClient();
                client.Connect(ip, port);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}