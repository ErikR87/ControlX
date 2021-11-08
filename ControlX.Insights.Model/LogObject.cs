namespace ControlX.Insights.Model;

public class LogObject
{
    public DateTimeOffset TimeGenerated { get; set; }
    public string Message { get; set; }
    public int SeverityLevel { get; set; }
    public string OperationName { get; set; }
    public string AppRoleInstance { get; set; }
}