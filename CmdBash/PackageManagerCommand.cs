using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdBash
{
    public class PackageManagerCommand : ICommand
    {
        public string CommandName => "pm";

        public Dictionary<string, Func<Dictionary<string, string>, List<string>>> SubCommands => new Dictionary<string, Func<Dictionary<string, string>, List<string>>>
        {
            ["install"] = Install,
        };

        private List<string> Install(Dictionary<string, string> args)
        {
            return new List<string>();
        }
    }
}
