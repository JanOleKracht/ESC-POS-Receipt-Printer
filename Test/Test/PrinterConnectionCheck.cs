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
        public static bool PrinterReachable(string ip, int port = 9100, int timeoutMs = 500)
        {
            using var client = new TcpClient();

            try
            {
                var result = client.BeginConnect(ip, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeoutMs));
                return success && client.Connected;
            }
            catch
            {
                return false;
            }
        }
    }
}