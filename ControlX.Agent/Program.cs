
using ControlX.Hub.Contract;
using ProtoBuf.Grpc.ClientFactory;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(builder =>
        {
            builder.AddApplicationInsights().AddSimpleConsole();
        });
        services.AddCodeFirstGrpcClient<IHubService>(o =>
        {
            o.Address = new Uri("https://localhost:8000");
        });
        services.AddHostedService<ControlX.Agent.HubClient.HubClient>();
        services.AddHostedService<ControlX.Agent.FileWatcher.FileWatcherWorker>();
        services.AddHostedService<ControlX.Agent.FTPWatcher.FTPWorker>();
        services.AddApplicationInsightsTelemetryWorkerService();
    })
    .Build();

await host.RunAsync();
