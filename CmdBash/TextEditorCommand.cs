using System;
using System.Collections.Generic;

namespace CmdBash
{
    public class TextEditorCommand : ICommand
    {
        public string CommandName => "te";

        public Dictionary<string, Func<Dictionary<string, string>, List<string>>> SubCommands => new Dictionary<string, Func<Dictionary<string, string>, List<string>>>()
        {
            ["edit"] = Edit,
        };

        public List<string> Help => new List<string> {

            "Sub commands :",
            "  - edit      Edit file",
            "Arguments :",
            "  --package-name, -p :    Package name",
        };

        private List<string> Edit(Dictionary<string, string> args)
        {

        }
    }
}
