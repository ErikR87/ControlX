using ControlX.Flow.Contract;
using Dahomey.Json.Attributes;
using Microsoft.Extensions.Logging;

namespace ControlX.Flow.Core;

[JsonDiscriminator(nameof(TestAction))]
public class TestAction : FlowAction<TestAction>, ITestAction
{
    ILogger<TestAction> _logger;

    public TestAction()
    {
    }

    public string Name => GetType().FullName;
    public IAutomate Automate { get; set; }

    public string Path { get; set; }

    public Task RunAsync()
    {
        using (_logger.BeginScope(this))
        {
            _logger.LogInformation($"Path {Automate.GetVariable<string>(Path)}");
            return Task.CompletedTask;
        }
    }
}