using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ControlX.Flow.Contract
{
    public interface IFTPAction : IAction
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SourceFile { get; set; }
    }
}
