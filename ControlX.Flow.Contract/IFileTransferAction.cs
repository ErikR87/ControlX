namespace ControlX.Flow.Contract;

public interface IFileTransferAction : IAction
{
    public string Agent { get; set; }
    public string FilePath { get; set; }
    public string NewFileName { get; set; }
}