using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlX.Insights.Models
{
    public class InsightsConfig
    {
        public string WorkspaceId { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }
}