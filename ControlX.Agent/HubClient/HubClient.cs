using ControlX.Hub.Contract;
using System.Runtime.CompilerServices;

namespace ControlX.Agent.HubClient;

public class HubClient : BackgroundService
{
    private readonly HubConfig _hubConfig;
    private readonly ILogger<HubClient> _logger;
    private readonly IHubService _hubService;

    private ConfiguredCancelableAsyncEnumerable<SubscribtionResponse> _subscribtion;

    public HubClient(IConfiguration configuration, ILogger<HubClient> logger, IHubService hubService)
    {
        _hubConfig = new HubConfig();
        configuration.GetSection("Hub").Bind(_hubConfig);
        _logger = logger;
        _hubService = hubService;

        if (_hubConfig.Url == null)
        {
            logger.LogWarning("no hub configuration found");
            return;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(_hubConfig.Url == null)
            return;

        try
        {
            _logger.LogInformation($"Connecting to hub {_hubConfig.Url}...");

            _subscribtion = _hubService.Subscribe().WithCancellation(stoppingToken);

            await foreach(var r in _subscribtion)
            {
                if (_hubService != null && r.Command != null)
                {
                    switch (r.Command)
                    {
                        case "Download":
                            await DownloadFile();
                            break;
                    }
                }
            }

            /*if (await Connect())
                _logger.LogInformation("Hub connected.");
            else
                _logger.LogWarning("No hub configuration found.");*/
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
        }
    }

    private async Task DownloadFile()
    {
        string tempFilename = Guid.NewGuid().ToString();
        string? filename = null;
        Console.WriteLine($"Download {filename}");
        using (var fs = new FileStream(tempFilename, FileMode.CreateNew))
        {
            await foreach (var chunk in _hubService.Download(new DownloadRequest()))
            {
                if (filename == null)
                    filename = chunk.DataChunkInfo.Filename;

                await fs.WriteAsync(chunk.DataChunkInfo.Data);
            }
        }

        if (filename != null)
            File.Move(tempFilename, filename);
    }
}
