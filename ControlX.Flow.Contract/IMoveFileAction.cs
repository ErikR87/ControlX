namespace ControlX.Flow.Contract;

public interface IMoveFileAction : IAction
{
    public string SourceFile { get; set; }
    public string DestinationSubFolder { get; set; }
    public string DestinationFileName { get; set; }
    public bool? Overwrite { get; set; }
}