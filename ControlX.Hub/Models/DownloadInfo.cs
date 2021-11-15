namespace ControlX.Hub.Models;

public class DownloadInfo
{
    public string Agent { get; set; }
    public string Filename {  get; set; }
    public byte[] Data {  get; set; }
}