using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CmdBash
{
    public class Variables
    {
        public class Config
        {
            [JsonProperty("i")]
            public List<string> InstalledPackages = new List<string>();

            public Config() { }
            public Config(List<string> InstalledPackages)
            {
                this.InstalledPackages = InstalledPackages;
            }
        }

        public static string Location;
        public static string LocationFormatted
        {
            get
            {
                var pattern = "(.):/";
                var match = Regex.Match(Location, pattern);
                if(match.Success)
                    return $"/{match.Groups[1]}/{string.Concat(Location.Skip(3))}";
                return Location;
            }
        }
        public static readonly string ConfigFileName = "wmconfig";
        public static string ConfigFullName => Location + ConfigFileName;
        public static readonly string ConfigDefaultContent = JsonConvert.SerializeObject(new Config(new List<string> { "WorkspaceManagerCommand", "PackageManagerCommand" }));
        public static Config Configs = new Config(new List<string> { "WorkspaceManagerCommand" });
        public static List<ICommand> Commands;
    }
}
