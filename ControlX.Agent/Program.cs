using ControlX.Agent;
using ControlX.Hub.Contract;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ControlX.Agent.FileWatcher.Worker>();
    })
    .Build();

await host.RunAsync();
