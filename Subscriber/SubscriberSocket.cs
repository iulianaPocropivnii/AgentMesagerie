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
        private readonly string _topic;
        private readonly byte[] _buffer;

        public SubscriberSocket(string topic)
        {
            _topic = topic;
            _buffer = new byte[ConnectionInfo.BUFFER_SIZE];
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ipAddress, int port)
        {
            try
            {
                _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectedCallback, null);
                Console.WriteLine("Connecting to the server.........");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to server: " + ex.Message);
            }
        }

        private void ConnectedCallback(IAsyncResult ar)
        {
            try
            {
                _socket.EndConnect(ar);

                if (_socket.Connected)
                {
                    Console.WriteLine("Subscriber connected to broker.");
                    Subscribe();
                    StartReceive();
                }
                else
                {
                    Console.WriteLine("Failed to connect to broker.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during connection callback: " + ex.Message);
            }
        }

        private void Subscribe()
        {
            try
            {
                string subscribeMessage = "subscribe#" + _topic;
                byte[] data = Encoding.UTF8.GetBytes(subscribeMessage);
                _socket.Send(data);
                Console.WriteLine($"Subscribed to topic: {_topic}");
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
                if (_socket == null || !_socket.Connected)
                    return;

                SocketError error;
                int received = _socket.EndReceive(ar, out error);

                if (received > 0 && error == SocketError.Success)
                {
                    byte[] payloadBytes = new byte[received];
                    Array.Copy(_buffer, payloadBytes, received);

                    PayloadHandler.Handle(payloadBytes);

                    // Continue receiving
                    _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
                }
                else
                {
                    Console.WriteLine("Disconnected from broker.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving data: " + ex.Message);
                Close();
            }
        }

        private void Close()
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
