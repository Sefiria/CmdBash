using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdBash
{
    public class CustomRepositoryManagerCommand : ICommand
    {
        public string CommandName => "crm";

        public Dictionary<string, Func<Dictionary<string, string>, List<string>>> SubCommands => new Dictionary<string, Func<Dictionary<string, string>, List<string>>>()
        {
            ["push"] = Push,
            ["pull"] = Pull,
        };

        public List<string> Help => new List<string> {
            "Sub commands :",
            "  - push      Push changes",
        };

        private List<string> Push(Dictionary<string, string> args)
        {
            if (!File.Exists(Variables.WorkspaceTokenFullName))
                return new List<string>() { "No Workspace found at current path." };



            return new List<string>() { $"Pushed." };
        }
        private List<string> Pull(Dictionary<string, string> args)
        {
            return new List<string>() { $"Pushed." };
        }


        private void UpdateChanges()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var remote = Path.Combine(appdata, "CmdBash");
            if (!Directory.Exists(remote)) Directory.CreateDirectory(remote);

            remote = Path.Combine(new string[] { remote, string.Concat(Variables.Location.Skip(3).Take(Variables.Location.Length - 4)).Replace('/', '.') });
            if (!Directory.Exists(remote))
            {
                Directory.CreateDirectory(remote);

                void RecursiveCopy(string loc, string _remote)
                {
                    IEnumerable<string> dirs = Directory.EnumerateDirectories(loc);
                    IEnumerable<string> files = Directory.EnumerateFiles(loc);

                    foreach (var file in files)
                        File.Copy(file, Path.Combine(_remote, Path.GetFileName(file)));
                    foreach (var dir in dirs)
                    {
                        Directory.CreateDirectory(Path.Combine(_remote, Path.GetFileName(dir)));
                        RecursiveCopy(Path.Combine(loc, dir), Path.Combine(_remote, Path.GetFileName(dir)));
                    }
                }

                RecursiveCopy(Variables.Location, remote);
            }
        }
    }
}
