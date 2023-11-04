// Create a project with the .NET 7 C# Console App template.
// Run 'dotnet add package System.IO.Ports --version 7.0.0' in the console.
// Replace the code in Program.cs with this code.

using System.IO.Ports;

public class Program
{
    static readonly DateTime currentDateTime = DateTime.Now;
    static readonly string filePath = @$".\SerialPortResponseFiles\serialPortResponse_{currentDateTime.ToString("yyyyMMddHHmmss")}.txt";
    static bool @continue;
    static SerialPort serialPort = new SerialPort();

    public static void Main()
    {
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(Read);
        GenerateResponseFile();

        // Create a new SerialPort object with default settings.
        serialPort = new SerialPort
        {
            PortName = SetPortName(serialPort.PortName),
            BaudRate = 9600,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
            Handshake = Handshake.XOnXOff,

            // Set the read/write timeouts
            ReadTimeout = 500,
            WriteTimeout = 500
        };

        serialPort.Open();
        @continue = true;
        readThread.Start();

        Console.WriteLine("Type QUIT to exit");

        while (@continue)
        {
            var inputValue = Console.ReadLine();

            if (stringComparer.Equals("quit", inputValue))
            {
                @continue = false;
            }
        }

        readThread.Join();
        serialPort.Close();
    }

    public static void Read()
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File {filePath} does not exist!");
        }
        else
        {
            using (StreamWriter sw = File.AppendText(filePath))
            {
                while (@continue)
                {
                    try
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        string readValue = serialPort.ReadLine();
                        sw.WriteLine($"[{dateTimeNow.ToString("yyyy-MM-ddTHH:mm:ss.fff")}] {readValue}");
                    }
                    catch (TimeoutException) { }
                }
            }
        }
    }

    /// <summary>
    /// Display Port values and prompt user to enter a port.
    /// </summary>
    /// <param name="defaultPortName">The default Port name.</param>
    /// <returns>The new Port name or the default if none is selected.</returns>
    public static string SetPortName(string defaultPortName)
    {
        Console.WriteLine("Available Ports:");
        foreach (string s in SerialPort.GetPortNames())
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
        var portName = Console.ReadLine();

        if (portName != null)
        {
            if (portName == "" || !portName.ToLower().StartsWith("com"))
            {
                portName = defaultPortName;
            }
        }
        return portName ?? defaultPortName;
    }

    public static void GenerateResponseFile()
    {
        if (!File.Exists(filePath))
        {
            using StreamWriter sw = File.CreateText(filePath);
        }
    }
}