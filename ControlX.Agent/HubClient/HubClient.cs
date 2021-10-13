using ControlX.Hub.Contract;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace ControlX.Agent.HubClient;

public class HubClient : BackgroundService
{
    private readonly HubConfig _hubConfig;

    public HubClient(IConfiguration configuration)
    {
        _hubConfig = new HubConfig();
        configuration.GetSection("Hub").Bind(_hubConfig);
    }

    public async Task Connect()
    {
        if (_hubConfig.Url != null)
        {
            using var channel = GrpcChannel.ForAddress(_hubConfig.Url);
            var service = channel.CreateGrpcService<IGreeterService>();
            var response = await service.SayHelloAsync(new HelloRequest { Name = "Agent" });
            Console.WriteLine($"from grpc: {response.Message}");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Connect();
    }
}
