using System;

namespace ControlX.Insights.Model
{
    public class LogObject
    {
        public DateTimeOffset TimeGenerated { get; set; }
        public string Message { get; set; }
        public int SeverityLevel { get; set; }
        public string OperationName { get; set; }
        public string AppRoleInstance { get; set; }
        public string Scope { get; set; }
    }

    public class LogObjectProps
    {
        public string AspNetCoreEnvironment { get; set; }
        public string CategoryName { get; set; }
        public string Scope { get; set; }

    }
}