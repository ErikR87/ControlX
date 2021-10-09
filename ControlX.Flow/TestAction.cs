using ControlX.Flow.Contract;

namespace ControlX.Flow;

public class TestAction : ITestAction
{
    public IAutomate Automate { get; set; }
    public string Name => "Test";

    public string Path { get; set; }

    public Task RunAsync()
    {
        Console.WriteLine($"Run {Name}.");
        Console.WriteLine($"Path {Automate.GetVariable<string>(Path)}");
        return Task.CompletedTask;
    }
}