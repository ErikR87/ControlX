namespace ControlX.Agent.FileWatcher;

public class Config
{
    public WorkerConfig[]? Worker { get; set; }
}

public class WorkerConfig
{
    public string? Path { get; set; }
}