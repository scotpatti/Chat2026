using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace ChatLibrary;

public class ChatNetworkModel: INotifyPropertyChanged
{
    #region Properties and Fields

    public readonly string IP = Extensions.LocalIpAddress().ToString();
    public readonly int PORT = 8888;
    
    private TcpClient? _socket;
    
    private string _Username = string.Empty;
    public string Username
    {
        get => _Username;
        set => SetField<string>(out _Username, value);
    }

    private string _MessageBoard;

    public string MessageBoard
    {
        get => _MessageBoard;
        set => SetField<string>(out _MessageBoard, value);
    }

    private string _CurrentMessage;

    public string CurrentMessage
    {
        get => _CurrentMessage;
        set => SetField<string>(out _CurrentMessage, value);
    }
    
    private bool _Connected;

    public bool Connected
    {
        get => _Connected;
        set => SetField<bool>(out _Connected, value);
    }

    #endregion
    
    #region Methods
    
    public void Connect()
    {
        _socket = new TcpClient();
        _socket.Connect(IP, PORT);
        Connected = true;
        Send();
        var thread = new Thread(ListenForMessages);
        thread.Start();
    }

    public void Send()
    {
        if (_socket != null)
        {
            ChatMessage msg = new ChatMessage(Username,  CurrentMessage);
            _socket.WriteChatMessage(msg);
        }
    }

    private void ListenForMessages()
    {
        while (_socket != null)
        {
            ChatMessage? msg = _socket.ReadChatMessage();
            if (msg != null)
            {
                MessageBoard += $"{msg.Sender} said: {msg.Message}{Environment.NewLine}";
            }
        }
    }
    
    #endregion
    
    #region INPC
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(out T field, T value, [CallerMemberName] string propertyName = "")
    {
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    #endregion
}