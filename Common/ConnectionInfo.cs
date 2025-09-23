using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ConnectionInfo
    {
        public const int BUFFER_SIZE = 1024;
        public byte[] Data; 
        public Socket Socket { get; set; }

        public string Address { get; set; }

        public string Topic { get; set; }

        public ConnectionInfo()
        {
            Data = new byte[BUFFER_SIZE];
        }

    }
}
