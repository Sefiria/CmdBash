using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CmdBash
{
    public class Variables
    {
        public class Config
        {
            [JsonProperty("i")]
            public ObservableCollection<string> InstalledPackages = new ObservableCollection<string>();

            public Config()
            {
                InstalledPackages.CollectionChanged += InstalledPackages_CollectionChanged;
            }

            public Config(List<string> InstalledPackages)
            {
                this.InstalledPackages = new ObservableCollection<string>(InstalledPackages);
                this.InstalledPackages.CollectionChanged += InstalledPackages_CollectionChanged;
            }

            private void InstalledPackages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                UpdateConfigs(e.NewItems);
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
        public static readonly string WorkspaceTokenFileName = ".wproj";
        public static string WorkspaceTokenFullName => Location + WorkspaceTokenFileName;
        public static string GetDefaultWorkspaceToken(string name) => JsonConvert.SerializeObject(new Project(name));
        public static string NoCmdPrefixToken = "¤";
        internal static Action ClearConsole = null;
        public static Action<KeyEventArgs> DefaultInputHandler = null;
        public static Action<KeyEventArgs> InputHandler = null;

        internal static void UpdateConfigs(System.Collections.IList items = null)
        {
            if(items == null)
                File.WriteAllText(ConfigFullName, JsonConvert.SerializeObject(Configs));
            else
                File.WriteAllText(ConfigFullName, JsonConvert.SerializeObject(new Config(items.Cast<string>().ToList())));
        }





        internal static char TokenFormat = 'ƒ';
        internal static ObservableCollection<string> Content = new ObservableCollection<string>();
        internal static readonly int MaxContentLength = 64;
        internal static List<string> History = new List<string>();
        internal static int HistoryCur = -1;
        internal static CursorObj CursorObj;
        internal static Stopwatch TimerTinkCursor = new Stopwatch();
        internal static string UserName;
    }
}
