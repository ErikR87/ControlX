using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlX.Flow.Contract
{
    public interface IHttpAction : IAction
    {
        public string Host { get; set; }
        public string RequestUri { get; set; }
        public HttpMethod Method { get; set; }
        public string Body { get; set; }

    }

    public enum HttpMethod
    {
        get,
        post
    }
}
