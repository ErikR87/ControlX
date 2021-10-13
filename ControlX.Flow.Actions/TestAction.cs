using ControlX.Flow.Contract;
using Dahomey.Json.Attributes;

namespace ControlX.Flow;

[JsonDiscriminator(nameof(TestAction))]
public class TestAction : ITestAction
{
    public string Name => GetType().FullName;
    public IAutomate Automate { get; set; }

    public string Path { get; set; }

    public Task RunAsync()
    {
        Console.WriteLine($"Path {Automate.GetVariable<string>(Path)}");
        return Task.CompletedTask;
    }
}