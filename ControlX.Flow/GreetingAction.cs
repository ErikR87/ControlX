using ControlX.Flow.Contract;
using ControlX.Hub.Contract;
using Dahomey.Json.Attributes;

namespace ControlX.Flow.Core;

[JsonDiscriminator(nameof(GreetingAction))]
public class GreetingAction : FlowAction<GreetingAction>, IGreetingAction
{
    public string Name { get; set; }
    public async Task RunAsync()
    {
        using (_logger.BeginScope(this))
        {
            await base.RunAsync();
            var result = await _hubService.SayHelloAsync(new HelloRequest { Name = Name });
            Console.WriteLine(result.Message);
        }
    }
}