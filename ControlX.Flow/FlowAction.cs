using ControlX.Flow.Contract;
using Microsoft.Extensions.Logging;

namespace ControlX.Flow.Core
{
    public abstract class FlowAction<T> : IAction
        where T : IAction
    {
        internal ILogger _logger;

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
    }
}
