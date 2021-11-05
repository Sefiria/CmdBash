using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CmdBash
{
    public class WorkspaceManagerCommand : ICommand
    {
        public string CommandName => "wm";

        public Dictionary<string, Func<Dictionary<string, string>, List<string>>> SubCommands => new Dictionary<string, Func<Dictionary<string, string>, List<string>>>
        {
            ["set"] = Set,
            ["ls"] = Ls,
            ["cd"] = Cd,
        };

        public List<string> Help => new List<string> {
            "Sub commands :",
            "  - set    Set current location as workspace",
            "  - ls     List current location elements",
            "  - cd     Move location in path",
            "Arguments :",
            "  --path, -p :    Relative path where to move",
        };

        private List<string> Set(Dictionary<string, string> args)
        {
            if (Variables.Location.CompareTo("~") == 0)
                return new List<string>() { "Workspace location not set, use 'cd' command." };

            if (File.Exists(Variables.ConfigFullName))
            {
                Variables.Configs = JsonConvert.DeserializeObject<Variables.Config>(File.ReadAllText(Variables.ConfigFullName));
                return new List<string>() { "Workspace loaded." };
            }

            Variables.Configs = JsonConvert.DeserializeObject<Variables.Config>(Variables.ConfigDefaultContent);
            Variables.UpdateConfigs();

            return new List<string>() { "Workspace set is successful." };
        }
        private List<string> Ls(Dictionary<string, string> args)
        {
            if (Variables.Location.CompareTo("~") == 0)
                return new List<string>() { "Workspace location not set, use 'cd' command." };

            List<string> directories = Directory.EnumerateDirectories(Variables.Location).ToList();
            List<string> files = Directory.EnumerateFiles(Variables.Location).ToList();

            var output = new List<string>();

            foreach (var directory in directories)
            {
                output.Add($"- {directory}    [DIRECTORY]");
            }

            foreach (var file in files)
            {
                output.Add($"- {file}    [FILE]");
            }

            return output;
        }
        private List<string> Cd(Dictionary<string, string> args)
        {
            var path = args.FirstOrDefault(x => new[] { "path", "p" }.Contains(x.Key)).Value;
            
            if (!string.IsNullOrWhiteSpace(path))
            {
                string[] nodes = Regex.Split(path, $@"(?<=/)");
                string winPath = "", winNode;
                bool pathDoesntExists = false;
                for(int i=0; i<nodes.Length; i++)
                {
                    string node = nodes[i];
                    if (i == 0)
                    {
                        if(Regex.IsMatch(string.Concat(path.Take(3)), "/./"))
                        {
                            i++;
                            winNode = $"{path[1]}:/";
                            if (!Directory.Exists(Path.GetPathRoot(winNode)))
                            {
                                pathDoesntExists = true;
                                break;
                            }
                            else
                            {
                                winPath += winNode;
                            }
                        }
                        else
                        {
                            if (node == "/")
                            {
                                winPath = Variables.Location;
                                continue;
                            }
                            winNode = $"{Variables.Location}{node}";
                            if (!Directory.Exists(winNode))
                            {
                                pathDoesntExists = true;
                                break;
                            }
                            else
                            {
                                winPath += winNode;
                            }

                        }
                    }
                    else
                    {
                        if (node == "../")
                        {
                            if (winPath.Split("/", true).Length < 2)
                            {
                                return new List<string>
                                    {
                                        "wm: cd: Path doesn't exists",
                                    };
                            }
                            var p = winPath.Replace("//", "/");
                            if (p.EndsWith("/")) p = string.Concat(p.Take(p.Length - 1));
                            winPath = Directory.GetParent(p).FullName.Replace("\\", "/");
                            continue;
                        }
                        winNode = node + "/";
                        if(!Directory.Exists(winPath + winNode))
                        {
                            pathDoesntExists = true;
                            break;
                        }
                        else
                        {
                            winPath += winNode;
                        }
                    }
                }

                if (pathDoesntExists)
                {
                    return new List<string>
                    {
                        "wm: cd: Path doesn't exists",
                    };
                }
                else
                {
                    Variables.Location = winPath.Replace("//", "/");
                    if (!Variables.Location.EndsWith("/")) Variables.Location += "/";
                    return new List<string>();
                }
            }

            return new List<string>();
        }
    }
}
