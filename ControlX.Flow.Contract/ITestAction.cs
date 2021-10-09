using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlX.Flow.Contract
{
    public interface ITestAction : IAction
    {
        public IAutomate Automate { get; set; }
        public string Name => "Test";

        public string Path { get; set; }
    }
}
