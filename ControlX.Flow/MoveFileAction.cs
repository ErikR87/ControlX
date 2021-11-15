using ControlX.Flow.Contract;
using Dahomey.Json.Attributes;
using Microsoft.Extensions.Logging;

namespace ControlX.Flow.Core
{
    [JsonDiscriminator(nameof(MoveFileAction))]
    public class MoveFileAction : FlowAction<MoveFileAction>, IMoveFileAction
    {
        public string SourceFile { get; set; }
        public string DestinationSubFolder { get; set; }
        public IAutomate Automate { get; set; }
        public bool? Overwrite { get; set; }
        public string DestinationFileName { get; set; }

        public async Task RunAsync()
        {
            using (_logger.BeginScope(this))
            {
                await base.RunAsync();

                if (SourceFile == null)
                    throw new ArgumentNullException("File");

                if (DestinationSubFolder == null)
                    throw new ArgumentNullException("DestinationFolder");

                var sourceParts = SourceFile.Split('\\');
                var sourceFolder = string.Join('\\', sourceParts.Take(sourceParts.Length - 1));

                var destinationPath = Path.Combine(sourceFolder, DestinationSubFolder);

                File.Move(SourceFile, $"{destinationPath}\\{DestinationFileName}", Overwrite.HasValue ? Overwrite.Value : false);

                _logger.LogInformation($"MoveFileAction: File {SourceFile} moved to {destinationPath}");
            }
        }
    }
}