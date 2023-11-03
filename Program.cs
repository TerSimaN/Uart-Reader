// Create a project with the .NET 7 C# Console App template.
// Run 'dotnet add package Microsoft.Windows.Compatibility --version 8.0.0-rc.2.23479.10' in the console.
// Replace the code in Program.cs with this code.

using System.IO.Ports;

public class PortReader
{
    static readonly string filePath = @".\serialPortResponse.txt";
    static readonly DateOnly dateOnly = DateOnly.FromDateTime(DateTime.Now);
    static readonly string currentDate = $"{dateOnly.Day}.{dateOnly.Month}.{dateOnly.Year}";
    static string contentToWrite = "";
    static bool @continue;
    static SerialPort serialPort = new SerialPort();

    public static void Main()
    {
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(Read);

        // Create a new SerialPort object with default settings.
        serialPort = new SerialPort
        {
            PortName = "COM3",
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
        while (@continue)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                TimeOnly timeOnly = TimeOnly.FromDateTime(dateTime);
                string timeOnlyHour = $"{((timeOnly.Hour < 10) ? $"0{timeOnly.Hour}" : $"{timeOnly.Hour}")}";
                string timeOnlyMinute = $"{((timeOnly.Minute < 10) ? $"0{timeOnly.Minute}" : $"{timeOnly.Minute}")}";
                string timeOnlySecond = $"{((timeOnly.Second < 10) ? $"0{timeOnly.Second}" : $"{timeOnly.Second}")}";
                string timeOnlyMillisecond = $"{((timeOnly.Millisecond < 100) && (timeOnly.Millisecond >= 10) ? $"0{timeOnly.Millisecond}" : (
                    (timeOnly.Millisecond < 10) ? $"00{timeOnly.Millisecond}" : $"{timeOnly.Millisecond}")
                )}";
                string currentTime = $"{timeOnlyHour}:{timeOnlyMinute}:{timeOnlySecond}.{timeOnlyMillisecond}";
                
                string readValue = serialPort.ReadLine();
                contentToWrite = $"[{currentDate}]:[{currentTime}] {readValue}";
                WriteToFile(contentToWrite);
            }
            catch (TimeoutException) { }
        }
    }

    public static void WriteToFile(string content)
    {
        if (!File.Exists(filePath))
        {
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(content);
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(content);
            }
        }
    }
}