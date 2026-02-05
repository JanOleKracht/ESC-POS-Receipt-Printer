using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core.Bonprinter
{
    public class TcpTransport
    {
        private readonly string _host;
        private readonly int _port;

        public TcpTransport(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Send(byte[] data)
        {
            try
            {
                using var client = new TcpClient();
                client.Connect(_host, _port);
                using NetworkStream? stream = client.GetStream();
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                throw new PrinterUnavailableException($"TCP-Connection Failed!-{_host}:{_port} not reachable");
            }
        }
    }
}