using ControlX.Flow.Contract;

namespace ControlX.Flow;

public interface IAction
{
    public IAutomate Automate { get; set; }

    public Task RunAsync();
}