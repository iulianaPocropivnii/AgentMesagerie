using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;

namespace Subscriber
{
    internal class SubscriberSocket
    {
        private Socket _socket;
        private readonly byte[] _buffer;

        public SubscriberSocket()
        {
            _buffer = new byte[ConnectionInfo.BUFFER_SIZE];
        }

        public void Connect(string ip, int port)
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(IPAddress.Parse(ip), port);
                if (_socket.Connected)
                    Console.WriteLine("Connected to the server.");
                StartReceive();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection error: " + ex.Message);
            }
        }
        public void Subscribe(string topic)
        {
            try
            {
                if (_socket == null || !_socket.Connected)
                {
                    Console.WriteLine("Socket not connected.");
                    return;
                }

                var subscribeMessage = "subscribe#" + topic;
                byte[] data = Encoding.UTF8.GetBytes(subscribeMessage);
                _socket.Send(data);
                Console.WriteLine($"Subscribed to topic: {topic}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending subscribe message: " + ex.Message);
            }
        }

        private void StartReceive()
        {
            try
            {
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error starting receive: " + ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int bytesRead = _socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    var payloadString = Encoding.UTF8.GetString(_buffer, 0, bytesRead);
                    Console.WriteLine($"Received payload: {payloadString}");
                    StartReceive();
                }
                else
                {
                    Console.WriteLine("Server closed connection.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Receive error: " + ex.Message);
                Close();
            }
        }

        public void Close()
        {
            try
            {
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _socket = null;
                    Console.WriteLine("Socket closed safely.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing socket: " + ex.Message);
            }
        }
    }
}
