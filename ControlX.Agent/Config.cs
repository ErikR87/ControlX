namespace ControlX.Agent;

public class HubConfig
{
    public string? Url { get; set; }
}

public class FileWatcherConfig
{
    public WorkerConfig[]? Worker { get; set; }
}

public class WorkerConfig
{
    public string? Path { get; set; }
    public string? Filter { get; set; }
    public bool? IncludeSubdirectories { get; set; }
    public string? Flow { get; set; }
}