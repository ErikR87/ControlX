using ControlX.Flow.Core;
using ControlX.Hub.Contract;
using Renci.SshNet.Sftp;

namespace ControlX.Agent.FTPWatcher;

public class FTPWorker : BackgroundService
{
    private readonly ILogger<FTPWorker> _logger;
    private readonly IHubService _hubService;
    private readonly IConfiguration _configuration;
    private readonly FTPWatcherConfig _ftpWatcherConfig;
    private readonly IList<SystemFTPWatcher> _watchers = new List<SystemFTPWatcher>();

    public FTPWorker(ILogger<FTPWorker> logger, IHubService hubService, IConfiguration configuration)
    {
        _logger = logger;
        _hubService = hubService;
        _configuration = configuration;
        _ftpWatcherConfig = new FTPWatcherConfig();
        _configuration.GetSection("FTPWatcher").Bind(_ftpWatcherConfig);
        InitFTPWatchers(_ftpWatcherConfig);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach(var watcher in  _watchers)
                {
                    watcher.Watch();
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.Message, ex.StackTrace);
        }

    }

    public void InitFTPWatchers(FTPWatcherConfig config)
    {

        if (config.Worker != null)
            foreach (var worker in config.Worker)
                _watchers.Add(
                    InitFileWatcher(worker)
                );
    }

    public SystemFTPWatcher InitFileWatcher(FTPWatcherWorkerConfig config)
    {
        if (config.Path == null)
            throw new NullReferenceException();

        var watcher = new SystemFTPWatcher(config, FTPWatcherEvents.FileAddEvent)
        {
            Host = config.Host,
            Port = config.Port,
            FingerPrint = config.FingerPrint,
            PassPhrase = config.PassPhrase,
            Password = config.Password,
            PasswordFile =config.PasswordFile,
            Path = config.Path,
            PrivateKeyFile = config.PrivateKeyFile,
            UserName = config.UserName,
            WatcherEvents = FTPWatcherEvents.FileAddEvent
        };

        watcher.OnFileAdded += (file) => OnCreated(file, config);

        //watcher.Error += OnError;

        return watcher;
    }

    private void OnCreated(SftpFile file, FTPWatcherWorkerConfig config)
    {
        _logger.LogInformation($"Event ftp file created: {file.FullName}");

        // Execute flow instance
        try
        {
            Automate.Execute(config.Flow, _logger, _hubService, file).Wait();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
        }

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
