class FileSerialPortDataWriterStrategy : ISerialPortDataWriterStrategy
{
    private StreamWriter streamWriter;

    public FileSerialPortDataWriterStrategy(string filePath)
    {
        streamWriter = File.CreateText(filePath);
    }

    public void Dispose()
    {
        streamWriter.Dispose();
    }

    public void Write(string line)
    {
        streamWriter.WriteLine("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"), line);
    }
}