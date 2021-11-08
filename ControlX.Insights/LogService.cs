using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using ControlX.Insights.Model;
using ControlX.Insights.Models;
using Microsoft.Extensions.Configuration;

namespace ControlX.Insights;

public class LogService
{
    private readonly InsightsConfig _config;

    public LogService(IConfiguration configuration)
    {
        _config = new InsightsConfig();
        configuration.GetSection("Insights").Bind(_config);
    }

    public async Task<IEnumerable<LogObject>> GetTraces(int daysBack = 30, int? top = 100)
    {
        var cred = new ClientSecretCredential(_config.Tenant, _config.ClientId, _config.Secret);
        var client = new LogsQueryClient(cred);

        var response = await client.QueryWorkspaceAsync(
            _config.WorkspaceId,
            $"AppTraces | {(top.HasValue ? ("top " + top.Value) : "")} by TimeGenerated",
            new QueryTimeRange(TimeSpan.FromDays(daysBack)));
        

        LogsTable table = response.Value.Table;

        var result = new List<LogObject>();

        foreach (var row in table.Rows)
            result.Add(new LogObject
            {
                TimeGenerated = (DateTimeOffset)row["TimeGenerated"],
                Message = (string)row["Message"],
                SeverityLevel = (int)row["SeverityLevel"],
                OperationName = (string)row["OperationName"],
                AppRoleInstance = (string)row["AppRoleInstance"]
            });

        return result;
    }

    
}