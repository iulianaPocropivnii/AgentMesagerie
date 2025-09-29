using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Common
{
    public class ConnectionInfo
    {
        public const int BUFFER_SIZE = 1024;
        public byte[] Data;
        public Socket Socket { get; set; }
        public string Address { get; set; }

        // Allow multiple topics per connection 
        public HashSet<string> Topics;

        public ConnectionInfo()
        {
            Data = new byte[BUFFER_SIZE];
            Topics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
