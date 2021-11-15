using ControlX.Flow.Contract;
using ControlX.Hub.Contract;
using Dahomey.Json.Attributes;

namespace ControlX.Flow.Core;

[JsonDiscriminator(nameof(FileTransferAction))]
public class FileTransferAction : FlowAction<FileTransferAction>, IFileTransferAction
{
    public string Agent { get; set; }
    public string FilePath { get; set; }
    public string NewFileName { get; set; }

    public async Task RunAsync()
    {
        using (_logger.BeginScope(this))
        {
            await base.RunAsync();
            var u = await _hubService.Upload(ReadFileAsync(FilePath));
        }
    }
    private async IAsyncEnumerable<DataChunk> ReadFileAsync(string path)
    {
        using (var fs = File.OpenRead(path))
        {
            var chunkSize = 1048576;
            var rest = (int)fs.Length;
            byte[] b = new byte[chunkSize];
            while (fs.Read(b, 0, b.Length) > 0)
            {
                await Task.Delay(10);

                rest -= chunkSize;

                yield return new DataChunk
                {
                    Agent = Agent,
                    DataChunkInfo = new DataChunkInfo
                    {
                        Filename = NewFileName,
                        Data = rest > 0 ? b : b.Take(-rest).ToArray()
                    }
                };
            }
        }
    }
}