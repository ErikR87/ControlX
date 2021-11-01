using System;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;

namespace ControlX.Insights;

public class Logs
{
   public Logs()
    {
        
    }

    public async Task Test()
    {
        string workspaceId = "DefaultWorkspace-4e4b85cc-97ea-4e71-948f-2e14a7438009-WEU";
        var client = new LogsQueryClient(new DefaultAzureCredential());
        Response<LogsQueryResult> response = await client.QueryWorkspaceAsync(
            workspaceId,
            "AzureActivity | top 10 by TimeGenerated",
            new QueryTimeRange(TimeSpan.FromDays(1)));

        LogsTable table = response.Value.Table;

        foreach (var row in table.Rows)
        {
            Console.WriteLine(row["OperationName"] + " " + row["ResourceGroup"]);
        }
    }
}