using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;   
using System.Threading.Tasks;
using System.Globalization;
using System.Net;
using System.Data;
using System.Net.NetworkInformation;
using Common;

namespace Broker
{
    class BrokerSocket
    {
        private const int CONNECTION_LIMIT = 8; 
        private Socket _socket;

        public BrokerSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(string ip, int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            _socket.Listen(CONNECTION_LIMIT);
            Accept();
        }

        private void Accept()
        {
            _socket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            ConnectionInfo connection = new ConnectionInfo();
            try
            {
                connection.Socket = _socket.EndAccept(ar);
                connection.Address = connection.Socket.RemoteEndPoint.ToString();
                connection.Socket.BeginReceive(connection.Data, 0, ConnectionInfo.BUFFER_SIZE, SocketFlags.None, ReciveCallback, connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error accepting connection: " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                Accept();
            }
        }

        private void ReciveCallback(IAsyncResult ar)
        {
            ConnectionInfo connection = ar.AsyncState as ConnectionInfo;
            try
            {
                Socket senderSocket = connection.Socket;
                SocketError response;
                int bufferSize = senderSocket.EndReceive(ar, out response);

                if (response == SocketError.Success)
                {
                    byte[] payload = new byte[bufferSize];
                    Array.Copy(connection.Data, payload, bufferSize);

                    PayloadHandler.Handle(payload, connection);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving data: " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                //on disconect method, 
                try
                {
                    connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length, SocketFlags.None, ReciveCallback, connection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error re-initiating receive: " + ex.Message);
                    var address = connection.Socket.RemoteEndPoint.ToString();
                    
                    ConnectionsStorage.RemoveConnection(address);
                    connection.Socket.Close();
                    Console.ReadLine();
                }
            }
        }

    }
}
