interface ISerialPortDataWriterStrategy : IDisposable
{
    public void Write(string line);
}