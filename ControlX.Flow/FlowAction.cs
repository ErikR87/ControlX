using ControlX.Flow.Contract;
using ControlX.Hub.Contract;
using Microsoft.Extensions.Logging;

namespace ControlX.Flow.Core
{
    public abstract class FlowAction<T> : IAction
        where T : IAction
    {
        internal ILogger _logger;
        internal IHubService _hubService;

        public IAutomate Automate { get; set; }
        
        public Task RunAsync()
        {
            _logger.LogInformation($"Running action {typeof(T).Name}");

            return Task.CompletedTask;
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void SetServices(IHubService hubService)
        {
            _hubService = hubService;
        }
    }
}
