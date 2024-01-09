class ConsoleSerialPortDataWriterStrategy : ISerialPortDataWriterStrategy
{
    public void Dispose() { }

    public void Write(string line)
    {
        Console.WriteLine("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"), line);
    }
}