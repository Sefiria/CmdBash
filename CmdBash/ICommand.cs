using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdBash
{
    public interface ICommand
    {
        string CommandName { get; }
        Dictionary<string, Func<Dictionary<string, string>, List<string>>> SubCommands { get; }

        List<string> Help { get; }
    }
}
