using ControlX.Flow.Contract;
using Dahomey.Json.Attributes;

namespace ControlX.Flow.Core
{
    [JsonDiscriminator(nameof(MoveFileAction))]
    public class MoveFileAction : IMoveFileAction
    {
        public string SourceFile { get; set; }
        public string DestinationFile { get; set; }
        public IAutomate Automate { get; set; }
        public bool? Overwrite { get; set; }

        public Task RunAsync()
        {
            if (SourceFile == null)
                throw new ArgumentNullException("File");

            if (DestinationFile == null)
                throw new ArgumentNullException("DestinationFolder");

            File.Move(SourceFile, DestinationFile, Overwrite.HasValue ? Overwrite.Value : false);

            return Task.CompletedTask;
        }
    }
}
