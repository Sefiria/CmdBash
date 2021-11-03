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
            ["uninstall"] = Uninstall,
        };

        public List<string> Help => new List<string> {
            "Sub commands :",
            "  - install      Install package",
            "  - uninstall    Uninstall package",
            "Arguments :",
            "  --package-name, -p :    Package name",
        };

        private List<string> Install(Dictionary<string, string> args)
        {
            var pck = args.FirstOrDefault(x => new[] { "package-name", "p" }.Contains(x.Key)).Value;

            if (string.IsNullOrWhiteSpace(pck))
                return new List<string> { "No package name informed." };
            var cmd = Variables.Commands.FirstOrDefault(x => x.CommandName == pck);
            if(cmd == null)
                return new List<string> { $"Package '{pck}' not found." };

            if(Variables.Configs.InstalledPackages.Contains(cmd.GetType().Name))
                return new List<string> { $"Package '{pck}' already installed." };

            Variables.Configs.InstalledPackages.Add(cmd.GetType().Name);

            return new List<string> { $"Package '{pck}' successfully installed." };
        }

        private List<string> Uninstall(Dictionary<string, string> args)
        {
            var pck = args.FirstOrDefault(x => new[] { "package-name", "p" }.Contains(x.Key)).Value;

            if (string.IsNullOrWhiteSpace(pck))
                return new List<string> { "No package name informed." };
            var cmd = Variables.Commands.FirstOrDefault(x => x.CommandName == pck);
            if (cmd == null)
                return new List<string> { $"Package '{pck}' not found." };

            if (!Variables.Configs.InstalledPackages.Remove(cmd.GetType().Name))
                return new List<string> { $"Package '{pck}' not installed." };

            return new List<string> { $"Package '{pck}' successfully uninstalled." };
        }
    }
}
