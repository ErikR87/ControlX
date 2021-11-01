namespace ControlX.Flow.Contract;
public interface IFTPAction : IAction
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Path { get; set; }
    public string UserName { get; set; }
    public string? Password { get; set; }
    public string? PasswordFile { get; set; }
    public string? PrivateKeyFile { get; set; }
    public string? PassPhrase { get; set; }
    public string? FingerPrint { get; set; }
    public string SourceFile { get; set; }
    public string? DownloadDestination { get; set; }
    public FTPMethod FTPMethod {  get; set; }
}

public enum FTPMethod
{
    Upload = 0,
    Download = 1
}