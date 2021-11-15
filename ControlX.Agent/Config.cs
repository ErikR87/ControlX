namespace ControlX.Agent;

public class HubConfig
{
    public string? Url { get; set; }
}

public class FileWatcherConfig
{
    public FileWatcherWorkerConfig[]? Worker { get; set; }
}

public class FileWatcherWorkerConfig
{
    public string? Path { get; set; }
    public string? Filter { get; set; }
    public bool? IncludeSubdirectories { get; set; }
    public string? Flow { get; set; }
}

public class FTPWatcherConfig
{
    public FTPWatcherWorkerConfig[]? Worker { get; set; }
}

public class FTPWatcherWorkerConfig
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

    public string? Flow { get; set; }
}