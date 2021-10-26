IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(builder =>
        {
            builder.AddApplicationInsights();
        });
        services.AddHostedService<ControlX.Agent.HubClient.HubClient>();
        services.AddHostedService<ControlX.Agent.FileWatcher.Worker>();
        services.AddApplicationInsightsTelemetryWorkerService();
    })
    .Build();

await host.RunAsync();
