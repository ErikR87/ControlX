
using ControlX.Hub.Contract;
using ProtoBuf.Grpc.ClientFactory;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
    .ConfigureServices(services =>
    {
        services.AddLogging(builder =>
        {
            builder.AddApplicationInsights().AddSimpleConsole();
        });
        services.AddCodeFirstGrpcClient<IHubService>(o =>
        {
            o.Address = new Uri("https://controlx-hub-dev.azurewebsites.net");
        });
        services.AddHostedService<ControlX.Agent.HubClient.HubClient>();
        services.AddHostedService<ControlX.Agent.FileWatcher.FileWatcherWorker>();
        services.AddHostedService<ControlX.Agent.FTPWatcher.FTPWorker>();
        services.AddApplicationInsightsTelemetryWorkerService();
    })
    .Build();

await host.RunAsync();
