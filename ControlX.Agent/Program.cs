using ControlX.Agent;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ControlX.Agent.FileWatcher.Worker>();
    })
    .Build();

await host.RunAsync();
