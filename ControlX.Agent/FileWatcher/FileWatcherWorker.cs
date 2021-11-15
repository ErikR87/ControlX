using ControlX.Flow.Core;
using ControlX.Hub.Contract;
using Microsoft.ApplicationInsights;

namespace ControlX.Agent.FileWatcher;

public class FileWatcherWorker : BackgroundService
{
    private readonly ILogger<FileWatcherWorker> _logger;
    private readonly IHubService _hubService;
    private readonly IConfiguration _configuration;
    private readonly FileWatcherConfig _fileWatcherConfig;
    private readonly IList<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

    public FileWatcherWorker(ILogger<FileWatcherWorker> logger, IHubService hubService, IConfiguration configuration)
    {
        _logger = logger;
        _hubService = hubService;
        _configuration = configuration;
        _fileWatcherConfig = new FileWatcherConfig();
        _configuration.GetSection("FileWatcher").Bind(_fileWatcherConfig);
        InitFTPWatchers(_fileWatcherConfig);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(1000, stoppingToken);
            }
        } catch (Exception ex)
        {
            _logger.LogCritical(ex.Message, ex.StackTrace);
        }

    }

    public void InitFTPWatchers(FileWatcherConfig config)
    {
        
        if(config.Worker != null)
            foreach(var worker in config.Worker)
                _watchers.Add(
                    InitFTPWatcher(worker)
                );
    }

    public FileSystemWatcher InitFTPWatcher(FileWatcherWorkerConfig config)
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

        watcher.Created += (sender, e) => OnCreated(sender, e, config);
        watcher.Changed += OnChanged;
            
        watcher.Error += OnError;

        if(config.Filter != null)
            watcher.Filter = config.Filter;

        if(config.IncludeSubdirectories.HasValue)
            watcher.IncludeSubdirectories = config.IncludeSubdirectories.Value;

        watcher.EnableRaisingEvents = true;

        return watcher;
    }

    private void OnCreated(object sender, FileSystemEventArgs e, FileWatcherWorkerConfig config)
    {
        _logger.LogInformation($"Event file created: {e.FullPath}");

        // Execute flow instance
        try
        {
            Automate.Execute(config.Flow, _logger, _hubService, sender, e).Wait();
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
        }
        
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
