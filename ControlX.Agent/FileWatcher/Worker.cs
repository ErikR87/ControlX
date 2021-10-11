using ControlX.Flow;
using ControlX.Flow.Core;
using ControlX.Hub.Contract;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace ControlX.Agent.FileWatcher;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IList<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        InitFileWatchers();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:8000");
        var service = channel.CreateGrpcService<IGreeterService>();
        var response = await service.SayHelloAsync(new HelloRequest { Name = "Agent" });

        Console.WriteLine($"from grpc: {response.Message}");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    public void InitFileWatchers()
    {
        var config = new Config();
        _configuration.GetSection("FileWatcher").Bind(config);

        if(config.Worker != null)
            foreach(var worker in config.Worker)
                _watchers.Add(
                    InitFileWatcher(worker)
                );
    }

    public FileSystemWatcher InitFileWatcher(WorkerConfig config)
    {
        if (config.Path == null)
            throw new NullReferenceException();

        var watcher = new FileSystemWatcher(config.Path);

        watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

        watcher.Created += OnCreated;
        watcher.Changed += OnChanged;
            
        watcher.Error += OnError;

        if(config.Filter != null)
            watcher.Filter = config.Filter;

        if(config.IncludeSubdirectories.HasValue)
            watcher.IncludeSubdirectories = config.IncludeSubdirectories.Value;

        watcher.EnableRaisingEvents = true;

        return watcher;
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        string value = $"Created: {e.FullPath}";
        Console.WriteLine(value);

        // create flow instance
        Automate.Execute(new IAction[]
        {
            new TestAction
            {
                Path = "$FileSystemEventArgs.FullPath"
            },
            new TestAction
            {
                Path = "Hallo Welt!"
            },
            new FTPAction
            {
                Host = "127.0.0.1",
                Port = 22,
                Path = "/",
                UserName = "tester",
                Password = "password",
                SourceFile = "$FileSystemEventArgs.FullPath"
            }
        }, sender, e).Wait();
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        Console.WriteLine($"Changed: {e.FullPath}");
    }

    private static void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private static void PrintException(Exception? ex)
    {
        if (ex != null)
        {
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine("Stacktrace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            PrintException(ex.InnerException);
        }
    }
}
