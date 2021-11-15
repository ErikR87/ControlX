using Grpc.Core;
using ProtoBuf.Grpc;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ControlX.Hub.Contract
{
    [DataContract]
    public class HelloReply
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }

    [DataContract]
    public class HelloRequest
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }
    }

    [DataContract]
    public class DownloadRequest
    {
    }

    [DataContract]
    public class UploadRequest
    {
        [DataMember(Order = 1)]
        public string Agent { get; set; }
        
        /*[DataMember(Order = 2)]
        byte[] Data { get; set; }*/
    }

    [DataContract]
    public class UploadResponse
    {

    }

    [DataContract]
    public class DataChunkInfo
    {
        [DataMember(Order = 1)]
        public string Filename { get; set; }
        [DataMember(Order = 2)]
        public byte[] Data { get; set; }
    }

    [DataContract]
    public class DataChunk
    {
        [DataMember(Order = 1)]
        public string Agent { get; set; }
        
        [DataMember(Order = 2)]
        public DataChunkInfo DataChunkInfo {  get; set; }
    }

    [DataContract]
    public class SubscribtionResponseArgs
    {
        [DataMember(Order = 1)]
        public string Agent {  get; set; }
    }

    [DataContract]
    public class SubscribtionResponse
    {
        [DataMember(Order = 1)]
        public string? Command { get; set; }
        [DataMember(Order = 2)]
        public SubscribtionResponseArgs? Args { get; set; }
    }

    [ServiceContract]
    public interface IHubService
    {
        [OperationContract]
        Task<HelloReply> SayHelloAsync(HelloRequest request,
            CallContext context = default);

        [OperationContract]
        IAsyncEnumerable<SubscribtionResponse> Subscribe(CallContext context = default);

        [OperationContract]
        IAsyncEnumerable<DataChunk> Download(DownloadRequest request, CallContext context = default);

        [OperationContract]
        ValueTask<UploadResponse> Upload(IAsyncEnumerable<DataChunk> data, CallContext context = default);
    }
}