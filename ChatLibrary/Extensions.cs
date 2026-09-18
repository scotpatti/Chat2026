using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ChatLibrary;

public static class Extensions
{
    public static string ReadString(this TcpClient client)
    {
        var sb = new StringBuilder();
        var bytes = new byte[client.ReceiveBufferSize];
        var stream = client.GetStream();
        bool EndOfMessageFound = false;
        while (!EndOfMessageFound)
        {
            stream.Read(bytes, 0, bytes.Length);
            string jMsg = Encoding.ASCII.GetString(bytes);
            if (jMsg.Contains("\0", StringComparison.Ordinal))
            {
                EndOfMessageFound = true;
                sb.Append(jMsg.Substring(0, jMsg.IndexOf("\0", StringComparison.Ordinal)));
            }
            else
            {
                sb.Append(jMsg);
            }
        }
        return sb.ToString();
    }

    public static void WriteString(this TcpClient client, string msg)
    {
        var stream = client.GetStream();
        var bytes = Encoding.ASCII.GetBytes(msg + "\0");
        stream.Write(bytes, 0, bytes.Length);
        stream.Flush();
    }

    public static ChatMessage? ReadChatMessage(this TcpClient client)
    {
        string msg = client.ReadString();
        (bool success, string errors) = JsonSchemaValidator.Validate(msg);
        if (success)
        {
            var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(msg);
            if (chatMessage == null)
            {
                throw new Exception("Something went wrong and we couldn't deserialize the object from JSON.");
            }
            return chatMessage;
        }

        return null;
    }

    public static void WriteChatMessage(this TcpClient client, ChatMessage msg)
    {
        string json = JsonConvert.SerializeObject(msg);
        client.WriteString(json);
    }

    public static IPAddress LocalIpAddress()
    {
        if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        {
            return IPAddress.None;
        }

        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        if (host != null)
        {
            var ip = host.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            if (ip != null)
                return ip;
        }
        return IPAddress.None;
    }
}