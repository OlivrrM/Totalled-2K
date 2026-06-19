using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Logger;

namespace t2kCore
{
    class Server
    {
        private TcpListener tcpListener;
        private Thread listenerThread;
        private bool isRunning = false;
        private int port = 1862;

        public Dictionary<TcpClient, Client> connectedClients = new Dictionary<TcpClient, Client>();

        public ServerInfo serverInfo = new ServerInfo();

        public World world;

        public ClientDataHandler clientDataHandler;

        public Server(int port)
        {
            this.port = port;
            tcpListener = new TcpListener(IPAddress.Any, port);

            //This should read from a file, hardcoded for now
            serverInfo.instance = "Dev";
            serverInfo.tps = 30;
        }

        public void Start()
        {
            isRunning = true;
            tcpListener.Start();
            ConsoleLogger.LogInfo($"Server started on port {port}");

            listenerThread = new Thread(ListenForClients);
            listenerThread.Start();

            world = new World(this);
            clientDataHandler = new ClientDataHandler(this);
        }

        public void Stop()
        {
            isRunning = false;
            tcpListener.Stop();
            listenerThread.Join();
            ConsoleLogger.LogInfo("Server stopped.");
        }

        private void ListenForClients()
        {
            while (isRunning)
            {
                try
                {
                    // Wait for a client connection
                    TcpClient client = tcpListener.AcceptTcpClient();
                    connectedClients.Add(client, new Client(client));
                    
                    OnClientConnected(client);

                    // Start a new thread to handle communication with the client
                    Thread clientThread = new Thread(() => ReceiveDataFromClient(client));
                    clientThread.Start();
                }
                catch (SocketException e)
                {
                    ConsoleLogger.LogError($"SocketException: {e.Message}");
                }
            }
        }
        public void OnClientConnected(TcpClient client)
        {
            ConsoleLogger.LogInfo($"Client connected with ip {client.Client.RemoteEndPoint.ToString()}");
            SendDataToClient(new PacketBatch
            {
                packets = new List<byte[]>
                {
                    PacketSerializer.SerializePacket(serverInfo)
                }
            }.Serialize()
            ,connectedClients[client].tcpClient);
        }

        private void ReceiveDataFromClient(TcpClient client)
        {
            byte[] message = new byte[256];
            int bytesRead;

            while (isRunning)
            {
                try
                {
                    // Read data from client
                    bytesRead = connectedClients[client].networkStream.Read(message, 0, message.Length);

                    if (bytesRead == 0)
                    {
                        // Client disconnected
                        DisconnectClient(client);
                        break;
                    }

                    clientDataHandler.HandleData(message, client);

                    // Convert bytes to string
                    //string dataReceived = Encoding.ASCII.GetString(message, 0, bytesRead);
                    //ConsoleLogger.LogInfo($"Received: {dataReceived}");
                }
                catch (Exception ex)
                {
                    ConsoleLogger.LogError($"Error receiving data from client [{client.Client.RemoteEndPoint.ToString()}]: {ex.Message}");
                    break;
                }
            }

            client.Close();
        }
        public void SendDataToClient(PacketBatch packetBatch,TcpClient tcpClient)
        {
            byte[] data = packetBatch.Serialize();
        }
        public void SendDataToClient(byte[] data, TcpClient tcpClient)
        {
            connectedClients[tcpClient].networkStream.Write(data, 0, data.Length);
        }
        public void DisconnectClient(TcpClient client)
        {
            world.RemovePlayer(connectedClients[client]);
            connectedClients.Remove(client);
            ConsoleLogger.LogInfo($"{client.Client.RemoteEndPoint.ToString()} disconnected");
        }
    }
}
