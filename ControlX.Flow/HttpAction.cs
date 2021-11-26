using ControlX.Flow.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlX.Utilities;
using Dahomey.Json.Attributes;

namespace ControlX.Flow.Core
{
    [JsonDiscriminator(nameof(HttpAction))]
    public class HttpAction : FlowAction<HttpAction>, IHttpAction
    {
        public string Host { get; set; }
        public string RequestUri { get; set; }
        public Contract.HttpMethod Method { get; set; }
        public string Body { get; set; }

        public async Task RunAsync()
        {
            using (_logger.BeginScope(this))
            {
                await base.RunAsync();

                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri(Host);

                HttpResponseMessage response = null;

                switch(Method)
                {
                    case Contract.HttpMethod.get:
                        response = await client.GetAsync(RequestUri);
                        break;
                    case Contract.HttpMethod.post:
                        var content = new StringContent(Body);
                        // RequestUri = "/receiveFile?dir=france&newFileName=customers.csv";
                        // RequestUri = "/receiveFile/frace";
                        response = await client.PostAsync(RequestUri, content);
                        break;
                }

                Automate.SetVariable(response.ToDictionary());
            }
        }
    }
}
