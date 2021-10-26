using ControlX.Hub.Contract;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace ControlX.Agent.HubClient;

public class HubClient : BackgroundService
{
    private readonly HubConfig _hubConfig;
    private readonly ILogger<HubClient> _logger;

    public HubClient(IConfiguration configuration, ILogger<HubClient> logger)
    {
        _hubConfig = new HubConfig();
        configuration.GetSection("Hub").Bind(_hubConfig);
        _logger = logger;
    }

    public async Task<bool> Connect()
    {
        if (_hubConfig.Url == null)
            return false;

        using var channel = GrpcChannel.ForAddress(_hubConfig.Url);
        var service = channel.CreateGrpcService<IGreeterService>();
        var response = await service.SayHelloAsync(new HelloRequest { Name = "Agent" });
        Console.WriteLine($"from grpc: {response.Message}");

        return true;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation($"Connecting to hub {_hubConfig.Url}...");
            if (await Connect())
                _logger.LogInformation("Hub connected.");
            else
                _logger.LogWarning("No hub configuration found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
        }
    }
}
