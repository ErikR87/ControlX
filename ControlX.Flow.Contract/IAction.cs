using ControlX.Flow.Contract;
using ControlX.Hub.Contract;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ControlX.Flow;

public interface IAction
{
    public IAutomate Automate { get; set; }

    public Task RunAsync();

    public void SetLogger(ILogger logger);

    public void SetServices(IHubService greeterService);

}