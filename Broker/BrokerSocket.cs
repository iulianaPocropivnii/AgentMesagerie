using System;
using System.Net;
using System.Net.Sockets;
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

        // validare IP & port + gestionare port ocupat
        public void Start(string ip, int port)
        {
            if (!IPAddress.TryParse(ip, out var ipAddress))
                throw new ArgumentException("IP invalid");
            if (port <= 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port invalid");

            try
            {
                _socket.Bind(new IPEndPoint(ipAddress, port));
                _socket.Listen(CONNECTION_LIMIT);
                Accept();
            }
            catch (SocketException se)
            {
                Console.WriteLine("Socket error on bind/listen: " + se.Message);
                throw;
            }
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
                connection.Address = connection.Socket?.RemoteEndPoint?.ToString() ?? "Unknown";

                connection.Socket.BeginReceive(
                    connection.Data, 0, ConnectionInfo.BUFFER_SIZE,
                    SocketFlags.None, ReciveCallback, connection
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error accepting connection: " + ex.Message);
            }
            finally
            {
                Accept(); 
            }
        }

        private void ReciveCallback(IAsyncResult ar)
        {
            ConnectionInfo connection = ar.AsyncState as ConnectionInfo;
            if (connection?.Socket == null) return;

            try
            {
                Socket senderSocket = connection.Socket;
                SocketError response;
                int bufferSize = senderSocket.EndReceive(ar, out response);

                // verificare dacă clientul s-a deconectat sau a apărut o eroare
                if (bufferSize == 0 || response != SocketError.Success)
                {
                    Console.WriteLine($"Client disconnected: {connection.Address}");
                    ConnectionsStorage.RemoveConnection(connection.Address);

                    try
                    {
                        if (connection.Socket.Connected)
                            connection.Socket.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                        // Socketul poate fi deja închis
                    }

                    connection.Socket.Close();
                    connection.Socket = null;
                    return;
                }

                // procesăm payload doar dacă avem date valide
                byte[] payload = new byte[bufferSize];
                Array.Copy(connection.Data, payload, bufferSize);

                PayloadHandler.Handle(payload, connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving data: " + ex.Message);

                try
                {
                    var address = connection.Socket?.RemoteEndPoint?.ToString();
                    if (!string.IsNullOrEmpty(address))
                    {
                        ConnectionsStorage.RemoveConnection(address);
                        Console.WriteLine($"Connection removed: {address}");
                    }

                    if (connection.Socket != null)
                    {
                        if (connection.Socket.Connected)
                            connection.Socket.Shutdown(SocketShutdown.Both);

                        connection.Socket.Close();
                        connection.Socket = null;
                    }
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine("Error during cleanup: " + cleanupEx.Message);
                }
            }
            finally
            {
                try
                {
                    if (connection.Socket != null && connection.Socket.Connected)
                    {
                        connection.Socket.BeginReceive(
                            connection.Data, 0, connection.Data.Length,
                            SocketFlags.None, ReciveCallback, connection
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error re-initiating receive: " + ex.Message);
                    var address = connection.Socket?.RemoteEndPoint?.ToString();

                    if (!string.IsNullOrEmpty(address))
                        ConnectionsStorage.RemoveConnection(address);

                    connection.Socket?.Close();
                    connection.Socket = null;
                }
            }
        }
    }
}
