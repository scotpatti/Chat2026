
using System.ComponentModel;
using ChatLibrary;

namespace ChatClient;

class Program
{
    private static string? Name { get; set; } = string.Empty;
    private static ChatNetworkModel? Model { get; set; }
    
    static void Main(string[] args)
    {
        var prog = new Program();
        prog.MainAsync();
    }

    public async void MainAsync()
    {
        Name = GetInput("Enter your name: ");
        Model = new ChatNetworkModel();

        Model.PropertyChanged += PropertyChangedListener;
        Model.Username = Name;
        Model.Connect();
        Console.WriteLine($"Connected to {Model.IP}:{Model.PORT}");
        Console.Write("Enter your message: ");

        await Task.Run(() => GetInputNonBlocking());
        while (true)
        {
            await Task.Delay(100);
        }
    }

    private static string GetInput(string prompt)
    {
        string? input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(input));

        return input;
    }

    private static void GetInputNonBlocking()
    {
        string tempMessage = string.Empty;
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Model!.CurrentMessage = tempMessage;
                    Model.Send();
                    tempMessage = string.Empty;
                    Console.Write($"{Environment.NewLine}Enter your message: ");
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (tempMessage.Length > 0)
                    {
                        tempMessage = tempMessage.Substring(0, tempMessage.Length - 1);
                    }
                }
                else
                {
                    tempMessage += key.KeyChar;
                    Console.Write(key.KeyChar);
                }

                Task.Delay(100).Wait();
            }
        }
    }

    private static void PropertyChangedListener(object sender, PropertyChangedEventArgs e)
    {
        if (sender is ChatNetworkModel model)
        {
            if (e.PropertyName == "MessageBoard" && !string.IsNullOrEmpty(model.MessageBoard))
            {
                Console.Clear();
                Console.WriteLine($"{Environment.NewLine}{model.MessageBoard}");
                Console.Write($"{Environment.NewLine}Enter your message: ");
            }
        }
    }
}