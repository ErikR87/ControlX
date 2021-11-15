using ControlX.Hub;
using ControlX.Hub.Contract;
using ControlX.Hub.Models;
using Grpc.Core;
using ProtoBuf.Grpc;
using System.Collections.Generic;

namespace ControlX.Hub.Services
{
    public class HubService : IHubService
    {
        private readonly ILogger<HubService> _logger;
        
        public static Dictionary<string, SubscribtionResponse?> Agents { get; set; } = new Dictionary<string, SubscribtionResponse?>();

        public static List<DownloadInfo> Downloads { get; set; } = new List<DownloadInfo>();

        public HubService(ILogger<HubService> logger)
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

        public async IAsyncEnumerable<SubscribtionResponse> Subscribe(CallContext context = default)
        {
            Console.Write("Subscribe");

            var host = context.ServerCallContext?.Host;

            if (host == null)
                throw new Exception("Hostname is missing");

            // register agent
            Agents.Add(host, null);
            Console.WriteLine("register " + host);

            while (context.ServerCallContext != null && !context.ServerCallContext.CancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("subRun...");
                Console.WriteLine(host);
                Console.WriteLine(Agents[host]);

                if(Agents.ContainsKey(host) && Agents[host] != null)
                {
                    var command = Agents[host];
                    Console.Write("send to " + host);
                    // send command response if exists
                    if(command != null)
                    {
                        yield return command;
                        Agents[host] = null;
                    }
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), context.CancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unsubscribe - connection lost");
                    Agents.Remove(host);
                }
            }

            Console.WriteLine("Unsubscribe");
            Agents.Remove(host);
        }

        public async IAsyncEnumerable<DataChunk> Download(DownloadRequest request, CallContext context = default)
        {
            Console.WriteLine("download...");
            var agent = context.ServerCallContext?.Host;

            if (agent == null)
                throw new Exception("Hostname is null");

            var dlInfos = Downloads.Where(x => x.Agent == agent);

            if (!dlInfos.Any())
                throw new Exception("Download info not found");

            var dlInfo = dlInfos.First();

            var chunkSize = 1048576;
            var rest = (int)dlInfo.Data.Length;

            for (var i = 0; i < dlInfo.Data.Length; i += chunkSize)
            {
                await Task.Delay(10);
                rest -= chunkSize;
                yield return new DataChunk { 
                    Agent = agent,
                    DataChunkInfo = new DataChunkInfo
                    {
                        Filename = dlInfo.Filename,
                        Data = dlInfo.Data.Skip(i).Take(rest > 0 ? chunkSize : -rest).ToArray()
                    }
                };
            }

            Downloads.Remove(dlInfo);

            if (dlInfos.Count() > 1)
                SetDownloadCommand(agent);
        }

        public async ValueTask<UploadResponse> Upload(IAsyncEnumerable<DataChunk> data, CallContext context = default)
        {
            Console.WriteLine("Upload");

            var bytes = new List<byte>();

            string? agent = null,
                filename = null;

            await foreach (var chunk in data)
            {
                if (agent == null)
                    agent = chunk.Agent;

                if (filename == null)
                    filename = chunk.DataChunkInfo.Filename;

                bytes.AddRange(chunk.DataChunkInfo.Data);
            }

            if (agent == null || filename == null)
                throw new Exception("agent or filename is null");

            Downloads.Add(new DownloadInfo
            {
                Agent = agent,
                Filename = filename,
                Data = bytes.ToArray()
            });

            SetDownloadCommand(agent);

            return new UploadResponse
            {

            };
        }

        private void SetDownloadCommand(string agent)
        {
            Agents[agent] = new SubscribtionResponse
            {
                Command = "Download",
                Args = new SubscribtionResponseArgs
                {
                    Agent = agent
                }
            };
        }
    }
}