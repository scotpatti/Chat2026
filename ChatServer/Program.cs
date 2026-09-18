using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;
using ChatLibrary;

namespace ChatServer;

public static class Program
{
    public static Dictionary<string, TcpClient> ClientList = new Dictionary<string, TcpClient>();
    
    public static void Main(string[] args)
    {
        var ServerSocket = new TcpListener(IPAddress.Any, 8888);
        ServerSocket.Start();
        Console.WriteLine("Server started on port 8888");
        while (true)
        {
            try
            {
                var ClientSocket = ServerSocket.AcceptTcpClient();
                ChatMessage? JoinMessage = ClientSocket.ReadChatMessage();
                if (JoinMessage != null)
                {
                    ClientList.Add(JoinMessage.Sender, ClientSocket);
                    Broadcast(new ChatMessage("System", $"{JoinMessage.Sender} has joined the chat"));
                    var client = new HandleClient(ClientSocket, JoinMessage.Sender);
                    client.StartClient();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Some client aborted connection: {e.Message}");
            }
        }
    }

    public static void Broadcast(ChatMessage msg)
    {
        foreach (var item in ClientList)
            try
            {
                item.Value.WriteChatMessage(msg);
            }
            catch
            {
                Console.WriteLine($"Unable to write to user {item.Key}, removing user");
                ClientList.Remove(item.Key);
            }
    }
}