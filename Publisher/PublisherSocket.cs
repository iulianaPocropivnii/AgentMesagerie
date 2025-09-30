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
            try
            {
                var result = _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), null, null);
                bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5), true);

                if (!success || !_socket.Connected)
                {
                    Console.WriteLine("Failed to connect to broker (timeout).");
                    IsConnected = false;
                    return;
                }

                Console.WriteLine("Connected to the broker.");
                IsConnected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection error: " + ex.Message);
                IsConnected = false;
            }
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

    }
}
