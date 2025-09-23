using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Publisher
{
    class PublisherSocket
    {
        private Socket _socket;
        public bool IsConnected;
        public PublisherSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ipAddress, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectedCallback, null);
            Thread.Sleep(2000); 

        }

        public void Send(byte[] data)
        {
            try
            {
                _socket.Send(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending data: " + ex.Message);
                Console.ReadLine();
            }
        }

        private void ConnectedCallback(IAsyncResult ar)
        {
            if(_socket.Connected)
            {
                Console.WriteLine("Connected to the server.");
            }
            else
            {
                Console.WriteLine("Failed to connect to the server.");
            }

            IsConnected = _socket.Connected;
        }
    }
}
