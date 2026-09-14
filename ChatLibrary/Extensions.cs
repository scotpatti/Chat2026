using System.Net.Sockets;
using System.Text;

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
        //TODO Start HERE
    }
}