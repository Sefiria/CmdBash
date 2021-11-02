using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdBash
{
    public class CustomRepositoryManagerCommand : ICommand
    {
        public string CommandName => "crm";

        public Dictionary<string, Func<Dictionary<string, string>, List<string>>> SubCommands => new Dictionary<string, Func<Dictionary<string, string>, List<string>>>();
    }
}
