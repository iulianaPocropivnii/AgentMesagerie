using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Broker
{
    static class ConnectionsStorage
    {
        private static List<ConnectionInfo> _connections;
        private static object _locker;

        static ConnectionsStorage()
        {
            _connections = new List<ConnectionInfo>();
            _locker = new object();
        }
        public static void AddOrUpdateConnection(ConnectionInfo connection)
        {
            lock (_locker)
            {
                var existing = _connections.FirstOrDefault(c => c.Address == connection.Address);
                if (existing == null)
                {
                    _connections.Add(connection);
                }
                else
                {
                    existing.Socket = connection.Socket ?? existing.Socket;

                    if (connection.Topics != null)
                    {
                        foreach (var t in connection.Topics)
                        {
                            existing.Topics.Add(t);
                        }
                    }
                }
            }
        }

        public static void RemoveConnection(string address)
        {
            lock (_locker)
            {
                var c = _connections.FirstOrDefault(x => x.Address == address);
                if (c != null)
                    _connections.Remove(c);
            }
        }

        public static List<ConnectionInfo> GetConnectionsByTopic(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                return new List<ConnectionInfo>();

            List<ConnectionInfo> selectedConnections;
            lock (_locker)
            {
                selectedConnections = _connections
                    .Where(x => x.Topics != null && x.Topics.Contains(topic))
                    .ToList();
            }
            return selectedConnections;
        }
        public static List<ConnectionInfo> GetAll()
        {
            lock (_locker)
            {
                return _connections.ToList();
            }
        }
    }
}
