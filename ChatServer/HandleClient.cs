using System.Net.Sockets;
using ChatLibrary;

namespace ChatServer;

internal class HandleClient
{
    private TcpClient clientSocket;
    private string clientName;
    
    public HandleClient(TcpClient clientSocket, string clientName)
    {
        if (clientSocket is null || string.IsNullOrEmpty(clientName))
        {
            throw new ArgumentNullException("Socket and name must not be null or empty");
        }
        this.clientSocket = clientSocket;
        this.clientName = clientName;
    }

    public void StartClient()
    {
        var thread = new Thread(DoChat);
        thread.Start();
    }

    private void DoChat()
    {
        while (true)
            try
            {
                var msg = clientSocket.ReadChatMessage();
                if (msg != null)
                {
                    if (msg.Sender == clientName)
                    {
                        Program.Broadcast(msg);
                        Console.WriteLine($"{clientName} said: {msg}");
                    }
                }
                else
                {
                    Console.WriteLine($"{clientName} did nott get a message because it was ill formed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                break;
            }
    }
}