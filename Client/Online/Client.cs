using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Totalled;

public class Client : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    public static bool online = false;
    public static OnlineState currentConnectionState = OnlineState.Unknown;
    private string serverIP = "127.0.0.1";
    private int serverPort = 1862;

    public bool OutputOutgoingStream;
    private void Awake()
    {
        Cache.client = this;
    }
    void OnApplicationQuit()
    {
        Disconnect();
    }
    public void ConnectToServer(string serverIP, int serverPort)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;

        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            online = true;
            Cache.terminal.Print("Connected to server.");
            currentConnectionState = OnlineState.OutGame;

            // Start a thread to listen for incoming messages
            receiveThread = new Thread(ReceiveDataFromServer);
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    private void ReceiveDataFromServer()
    {
        byte[] receivedBytes = new byte[256];
        int byteCount;

        while (online)
        {
            // Check if there is data available before trying to read
            if (stream.DataAvailable)
            {
                receivedBytes = new byte[128]; // Wipe the buffer so reading from it for debugging purposes is easier
                byteCount = stream.Read(receivedBytes, 0, receivedBytes.Length);

                if (byteCount == 0)
                {
                    // The server has closed the connection
                    Cache.terminal.Print("Server closed");
                    online = false;
                    break;
                }

                MainThreadDispatcher.Enqueue(() =>{ //Run process code on main Unity thread so Unity specific calls such as Instantiate will work
                    Cache.onlineProxyIn.ProcessReceivedData(receivedBytes, byteCount);
                });

                // Convert byte array to string
                //string dataReceived = Encoding.ASCII.GetString(receivedBytes, 0, byteCount);
                //Cache.terminal.Print("Received message from server: " + dataReceived);
            }
            else
            {
                // Give up control to other threads to avoid blocking
                Thread.Sleep(10);
            }
            /*
            try
            {
                // Check if there is data available before trying to read
                if (stream.DataAvailable)
                {
                    byteCount = stream.Read(receivedBytes, 0, receivedBytes.Length);

                    if (byteCount == 0)
                    {
                        // The server has closed the connection
                        Cache.terminal.Print("Server closed");
                        online = false;
                        break;
                    }

                    Cache.onlineProxyIn.ProcessReceivedData(receivedBytes, byteCount);
                    // Convert byte array to string
                    //string dataReceived = Encoding.ASCII.GetString(receivedBytes, 0, byteCount);
                    //Cache.terminal.Print("Received message from server: " + dataReceived);
                }
                else
                {
                    // Give up control to other threads to avoid blocking
                    Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving data from server: " + e.Message);
                online = false;
                break;
            }
            */
        }
    }
    public void SendDataToServer(byte[] data)
    {
        if (OutputOutgoingStream) { Cache.terminal.Print($"Sending stream of bytes to server: [{Utilities.ToReadableByteArray(data)}]"); }
        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending data to server: {e.Message}");
        }
    }

    public void SendMessageToServer(string message) //Deprecated. Server will not take anything from just a string
    {
        if (online && client != null && client.Connected)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Cache.terminal.Print("Sent message to server: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError("Error sending message to server: " + e.Message);
            }
        }
    }

    public void Disconnect()
    {
        online = false;
        receiveThread?.Join();
        stream?.Close();
        client?.Close();
        Cache.terminal.Print("Disconnected from server.");
        currentConnectionState = OnlineState.Disconnected;
    }
}
