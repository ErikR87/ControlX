IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ControlX.Agent.HubClient.HubClient>();
        services.AddHostedService<ControlX.Agent.FileWatcher.Worker>();
    })
    .Build();

await host.RunAsync();
