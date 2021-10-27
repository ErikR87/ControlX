using ControlX.Flow.Contract;
using Microsoft.Extensions.Logging;

namespace ControlX.Flow;

public interface IAction
{
    public IAutomate Automate { get; set; }

    public Task RunAsync();

    public void SetLogger(ILogger logger);
}