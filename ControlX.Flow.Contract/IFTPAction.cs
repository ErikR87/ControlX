namespace ControlX.Flow.Contract;
public interface IFTPAction : IAction
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Path { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string SourceFile { get; set; }
}