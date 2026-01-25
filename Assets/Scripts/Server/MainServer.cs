using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class MainServer : MonoBehaviour
{
    [Header("Connection Settings")]
    [SerializeField] public string serverIP = "127.0.0.1";
    [SerializeField] public int serverPort = 8080;
    [SerializeField] public string playerName = "TestPlayer";
    [SerializeField] public string playerDeck = "CITIZEN,CITIZEN,CITIZEN";

    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;
    private CancellationTokenSource cts;
    private bool isConnected = false;

    void Start()
    {
        ConnectToServer();
    }

    void OnDestroy()
    {
        Disconnect();
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    public async void ConnectToServer()
    {
        if (isConnected)
        {
            Debug.LogWarning("Already connected to server.");
            return;
        }

        try
        {
            // Connect to server
            client = new TcpClient();
            await client.ConnectAsync(serverIP, serverPort);
            stream = client.GetStream();
            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            // Send initial connection message
            SendServerMessage(new[] { "player", "connect" },
                $"||PLAYER.NAME||{playerName}||PLAYER.DECK||{playerDeck}||");

            // Set timeout for receiving responses
            client.Client.ReceiveTimeout = 5000;

            isConnected = true;
            Debug.Log($"Connected to server at {serverIP}:{serverPort}");

            // Start listening for server responses
            cts = new CancellationTokenSource();
            _ = ListenForServerResponses(cts.Token);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket error: {ex.Message}");
            isConnected = false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Connection error: {ex.Message}");
            isConnected = false;
        }
    }

    public void Disconnect()
    {
        if (!isConnected) return;
        try
        {
            if (writer != null && client?.Connected == true)
            {
                try
                {
                    string header = string.Join("||HEADER.SEP||", new[] { "player", "disconnect" }) + "||HEADER.END||";
                    writer.WriteLine(header + playerName);
                    writer.Flush();
                    Debug.Log("[Sent]: Disconnect message");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Could not send disconnect message: {ex.Message}");
                }
            }
            cts?.Cancel();
            writer?.Close();
            reader?.Close();
            stream?.Close();
            client?.Close();
            isConnected = false;
            Debug.Log("Disconnected from server.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during disconnect: {ex.Message}");
        }
    }
    // Continuously listen for responses from the server
    private async Task ListenForServerResponses(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && isConnected)
            {
                var response = await reader.ReadLineAsync();
                if (response != null)
                {
                    Debug.Log($"[Server]: {response}");
                    // You can add event callbacks here to notify UI or other systems
                    OnServerMessageReceived(response);
                }
                else
                {
                    // Server disconnected
                    Debug.LogWarning("Server closed connection.");
                    isConnected = false;
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            if (isConnected)
            {
                Debug.LogError($"[Server listener error]: {ex.Message}");
                isConnected = false;
            }
        }
    }

    // Override this method or use events to handle server messages
    protected virtual void OnServerMessageReceived(string message)
    {

    }

    // Public methods to call from UI or other scripts
    public void CreateLobby(string lobbyName)
    {
        if (!isConnected)
        {
            Debug.LogWarning("Not connected to server.");
            return;
        }
        SendServerMessage(new[] { "game", "lobby", "create" }, lobbyName);
    }

    public void ListLobbies()
    {
        if (!isConnected)
        {
            Debug.LogWarning("Not connected to server.");
            return;
        }
        SendServerMessage(new[] { "game", "lobby", "list" }, "");
    }

    public void JoinLobby(string lobbyId)
    {
        if (!isConnected)
        {
            Debug.LogWarning("Not connected to server.");
            return;
        }
        SendServerMessage(new[] { "game", "lobby", "join" }, lobbyId);
    }

    public void StartGame()
    {
        if (!isConnected)
        {
            Debug.LogWarning("Not connected to server.");
            return;
        }
        SendServerMessage(new[] { "game", "lobby", "start" }, "");
    }

    public void SendCustomMessage(string[] headers, string message)
    {
        if (!isConnected)
        {
            Debug.LogWarning("Not connected to server.");
            return;
        }
        SendServerMessage(headers, message);
    }

    private void SendServerMessage(string[] headers, string message)
    {
        try
        {
            string header = string.Join("||HEADER.SEP||", headers) + "||HEADER.END||";
            writer.WriteLine(header + message);
            Debug.Log($"[Sent]: {header}{message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending message: {ex.Message}");
            isConnected = false;
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }
}
