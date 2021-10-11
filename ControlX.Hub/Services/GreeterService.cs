using ControlX.Hub;
using ControlX.Hub.Contract;
using Grpc.Core;
using ProtoBuf.Grpc;

namespace ControlX.Hub.Services
{
    public class GreeterService : IGreeterService
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public Task<HelloReply> SayHelloAsync(HelloRequest request, CallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}