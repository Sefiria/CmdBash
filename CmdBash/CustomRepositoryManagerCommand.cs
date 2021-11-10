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
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var remote = Path.Combine(appdata, "CmdBash");
            if (!Directory.Exists(remote))
                return new List<string>() { "No remote set (any workspace), push failed." };

            remote = Path.Combine(new string[] { remote, string.Concat(Variables.Location.Skip(3).Take(Variables.Location.Length - 4)).Replace('/', '.') });
            if (!Directory.Exists(remote))
                return new List<string>() { "No Workspace set, push failed." };

            var result = DoChanges(Variables.Location, remote);
            result.Add("Pushed.");
            return result;
        }
        private List<string> Pull(Dictionary<string, string> args)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var remote = Path.Combine(appdata, "CmdBash");
            if (!Directory.Exists(remote))
                return new List<string>() { "No remote set (any workspace), push failed." };

            remote = Path.Combine(new string[] { remote, string.Concat(Variables.Location.Skip(3).Take(Variables.Location.Length - 4)).Replace('/', '.') });
            if (!Directory.Exists(remote))
                return new List<string>() { "No Workspace set, push failed." };

            var result = DoChanges(remote, Variables.Location);
            result.Add("Pulled.");
            return result;
        }

        private List<string> DoChanges(string local, string remote)
        {
            List<string> foldersToCreate = new List<string>();
            List<string> foldersToRemove = new List<string>();
            Dictionary<string, string> filesToCreate = new Dictionary<string, string>();
            Dictionary<string, string> modifiedFiles = new Dictionary<string, string>();
            void Compare(string _local, string _remote)
            {
                var localFile = File.ReadAllText(_local);

                if (!File.Exists(_remote))
                {
                    filesToCreate[_remote] = localFile;
                    return;
                }

                var remoteFile = File.ReadAllText(_remote);
                if (localFile.CompareTo(remoteFile) != 0)
                    modifiedFiles[_remote] = localFile;
            }

            void RecursiveCompare(string loc, string _remote)
            {
                IEnumerable<string> dirs = Directory.EnumerateDirectories(loc);
                IEnumerable<string> files = Directory.EnumerateFiles(loc);

                foreach (var file in files)
                    Compare(file, Path.Combine(_remote, Path.GetFileName(file)));
                foreach (var dir in dirs)
                {
                    if (!Directory.Exists(Path.Combine(_remote, Path.GetFileName(dir))))
                        foldersToCreate.Add(Path.Combine(_remote, Path.GetFileName(dir)));

                    RecursiveCompare(Path.Combine(loc, dir), Path.Combine(_remote, Path.GetFileName(dir)));
                }
                if (Directory.Exists(_remote))
                {
                    var remoteFolders = Directory.EnumerateDirectories(_remote).ToList();
                    remoteFolders.RemoveAll(x => dirs.Contains(x));
                    if (remoteFolders.Count > 0)
                        foldersToRemove.AddRange(remoteFolders);
                }
            }

            RecursiveCompare(local, remote);


            foreach (var folder in foldersToCreate)
                Directory.CreateDirectory(folder);
            foreach (var folder in foldersToRemove)
                Directory.Delete(folder, true);
            foreach (var file in filesToCreate)
                File.WriteAllText(file.Key, file.Value);
            foreach (var file in modifiedFiles)
                File.WriteAllText(file.Key, file.Value);

            return new List<string>() {
                $"{foldersToCreate.Count + foldersToRemove.Count} folders comitted ( ƒ2{foldersToCreate.Count} created, ƒ1{foldersToRemove.Count} removed )",
                $"{filesToCreate.Count + modifiedFiles.Count} files comitted ( ƒ2{filesToCreate.Count} created, ƒ3{modifiedFiles.Count} overwritten, ƒ1{modifiedFiles.Count} removed )" };
        }
    }
}
