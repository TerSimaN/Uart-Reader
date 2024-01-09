using System.IO.Ports;

public class Program
{
    static private Thread? readThread;
    static private SerialPort serialPort = new SerialPort();
    static private ISerialPortDataWriterStrategy? serialPortDataWriterStrategy;
    static private bool @continue;
    static private string? portName;

    /// <summary>
    /// Entrypoint for the application.
    /// </summary>
    /// <param name="args">Command-line arguments i.e. [<COMn>] [<OutputFile>]</param>
    public static void Main(string[] args)
    {
        ConsoleKeyInfo consoleKeyInfo;
        readThread = new Thread(Read);

        if (args.Length < 1)
        {
            portName = SetPortName(serialPort.PortName);
            serialPortDataWriterStrategy = new ConsoleSerialPortDataWriterStrategy();
        }
        else
        {
            portName = args[0];
            serialPortDataWriterStrategy = args.Length < 2 ? 
                new ConsoleSerialPortDataWriterStrategy() : 
                new FileSerialPortDataWriterStrategy(args[1]);
        }

        // Create a new SerialPort object with default settings.
        serialPort = new SerialPort
        {
            PortName = portName,
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

        Console.WriteLine("Press 'X' to exit, or CTRL+C to interrupt the program.");

        Console.CancelKeyPress += new ConsoleCancelEventHandler(OnKeyPress);
        while (@continue)
        {
            consoleKeyInfo = Console.ReadKey(true);

            Console.WriteLine($"  Key pressed: {consoleKeyInfo.Key}");

            if (consoleKeyInfo.Key == ConsoleKey.X)
            {
                @continue = false;
            }
        }

        readThread.Join();
        serialPort.Close();
    }

    /// <summary>
    /// Handles the interruption of the program from pressing CTRL+C.
    /// </summary>
    protected static void OnKeyPress(object? sender, ConsoleCancelEventArgs args)
    {
        Console.WriteLine("The program has been interrupted.");
        if (!args.Cancel)
        {
            @continue = false;
            readThread?.Join();
            serialPort.Close();
        }
    }

    /// <summary>
    /// Reads from the serial port and writes read content to a file.
    /// </summary>
    public static void Read()
    {
        while (@continue)
        {
            try
            {
                string readValue = serialPort.ReadLine();
                serialPortDataWriterStrategy?.Write(readValue);
            }
            catch (TimeoutException) { }
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
}