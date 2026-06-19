using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace t2kCore
{
    class Client
    {
        public TcpClient tcpClient;
        public NetworkStream networkStream;
        public Player player;
        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            networkStream = tcpClient.GetStream();
        }
    }
}
