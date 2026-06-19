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
    /// Main problems that need to be addressed for this program:
    
    // Packet validation
    // Clients need to validate that they have received all packets up to that tick and are all in contact, if not, client requests new packets sent.

    // More compact packets
    // Packets need to be more compact than a json file.

    // Client proxy
    // A funnel for all data to go through to then be processed and sent to server, without having to rewrite loads of client code
    class Program
    {
        public static Server server = null;
        static void Main(string[] args)
        {
            server = new Server(1862);
            server.Start();
        }
    }
}
