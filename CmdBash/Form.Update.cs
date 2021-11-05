using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CmdBash
{
    public partial class Form : System.Windows.Forms.Form
    {
        char TokenFormat = 'ƒ';
        ObservableCollection<string> Content = new ObservableCollection<string>();
        const int MaxContentLength = 64;
        List<string> History = new List<string>();
        int HistoryCur = -1;
        CursorObj CursorObj;
        Stopwatch TimerTinkCursor = new Stopwatch();
        string UserName;


        private void UpdateInit()
        {
            HeaderUpdateInit();

            Variables.Location = "~";

            var type = typeof(ICommand);
            Variables.Commands = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface)
                .Select(x => (ICommand)Activator.CreateInstance(x))
                .ToList();

            UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            CursorObj = new CursorObj(0, 0);
            Content.CollectionChanged += (object s, NotifyCollectionChangedEventArgs e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (CursorObj.Y - ScrollValue < 0 || CursorObj.Y - ScrollValue > 27)
                        ScrollValue = CursorObj.Y;

                    if (Content.Count > MaxContentLength)
                    {
                        Content.RemoveAt(0);
                        CursorObj.Y--;
                    }
                }
            };
            Content.Add($"ƒ2{UserName} ƒ4MINGW64 ƒ5{Variables.LocationFormatted}");
            Content.Add($"$ ");
            CursorObj.X = Content[Content.Count - 1].Length;
            CursorObj.Y = Content.Count - 1;

            TimerTinkCursor.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            if (Content.FirstOrDefault(x => x.Contains(CR)) != null)
            {
                List<string> content = new List<string>(Content);
                Content.Clear();
                foreach (string line in content)
                {
                    Content = Content.AddRange(line.Split(new[] { CR }, StringSplitOptions.None));
                }
            }

            if (TimerTinkCursor.ElapsedMilliseconds >= 1000)
                TimerTinkCursor.Restart();
        }

        private void CursorObjNextLine(int nblines = 1)
        {
            CursorObj.Y += nblines;
            CursorObj.X = 2;
        }

        private string GetUnformattedLine(string line)
        {
            for(int i=0; i<line.Length; i++)
            {
                if(line[i] == TokenFormat)
                {
                    line = string.Concat(line.Take(i).Concat(line.Skip(i+1)));
                }
            }
            return line;
        }
        private Color GetColor(int c) => GetColorFromFormat($"{c}"[0]);
        private Color GetColorFromFormat(char value)
        {
            if(int.TryParse($"{value}", out int v))
            {
                switch(v)
                {
                    default:
                    case 0: return Color.White;
                    case 1: return Color.Red;
                    case 2: return Color.Lime;
                    case 3: return Color.Blue;
                    case 4: return Color.Magenta;
                    case 5: return Color.Yellow;
                    case 6: return Color.Gray;
                    case 7: return Color.DimGray;
                }
            }
            else
            {
                return Color.White;
            }
        }


        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            //Console.WriteLine(e.KeyValue);

            if (e.KeyValue == 38)
            {
                if(History.Count > 0 && HistoryCur >= 0 && HistoryCur < History.Count)
                {
                    Content[CursorObj.Y] = History[HistoryCur--];
                    if (HistoryCur < 0) HistoryCur = 0;
                    CursorObj.X = Content[CursorObj.Y].Length;
                }
                return;
            }
            if (e.KeyValue == 40)
            {
                if (History.Count > 0 && HistoryCur >= 0 && HistoryCur < History.Count)
                {
                    Content[CursorObj.Y] = History[HistoryCur++];
                    if (HistoryCur >= History.Count) HistoryCur = History.Count - 1;
                    CursorObj.X = Content[CursorObj.Y].Length;
                }
                return;
            }
            if (e.KeyValue == 37 && CursorObj.X > 2)
            {
                CursorObj.X--;
                return;
            }
            if (e.KeyValue == 39 && CursorObj.X < Content[CursorObj.Y].Length)
            {
                CursorObj.X++;
                return;
            }


            if ((e.KeyValue >= 65 && e.KeyValue <= 90)
                || (e.KeyValue >= 96 && e.KeyValue <= 105)
                || (e.Shift && e.KeyValue >= 48 && e.KeyValue <= 57)
                || (!e.Shift && e.KeyValue == 223)
                || new List<int> { 8, 13, 32, 54, 106, 107, 109, 111, 187, 188, 190, 191, 220 }.Contains(e.KeyValue))
            {
                string v = $"{(char)e.KeyValue}";
                if (e.KeyValue >= 96 && e.KeyValue <= 105)
                    v = $"{e.KeyValue - 96}";
                if (e.Shift && e.KeyValue >= 48 && e.KeyValue <= 57)
                    v = $"{e.KeyValue - 48}";

                switch (e.KeyValue)
                {
                    case 111: v = "/"; break;
                    case 191: v = e.Shift ? "/" : ":"; break;
                    case 106: v = "*"; break;
                    case 109: v = "-"; break;
                    case 54: if(!e.Shift) v = "-"; break;
                    case 107: v = "+"; break;
                    case 187: v = e.Shift ? "+" : "="; break;
                    case 220: v = "*"; break;
                    case 188: v = e.Shift ? "?" : ","; break;
                    case 190: v = e.Shift ? "." : ";"; break;
                    case 223: v = "!"; break;
                }

                switch (e.KeyValue)
                {
                    case 8:
                        if (CursorObj.X > 2)
                        {
                            Content[Content.Count - 1] = Content[Content.Count - 1].Remove(CursorObj.X - 1, 1);
                            CursorObj.X--;
                        }
                        break;

                    case 13:
                        if (CursorObj.X > 2)
                        {
                            if (History.Count > 10)
                                History.RemoveAt(0);
                            else
                                HistoryCur++;
                            History.Add(Content[CursorObj.Y]);
                            int nblines = Execute(Content[Content.Count - 1]);
                            if (nblines > -1)
                            {
                                nblines += 3;
                                Content.Add("");
                                Content.Add($"ƒ2{UserName} ƒ4MINGW64 ƒ5{Variables.LocationFormatted}");
                                Content.Add($"$ ");
                                CursorObjNextLine(nblines);
                            }
                        }
                        break;

                    default:
                        if(CursorObj.X == Content[Content.Count - 1].Length)
                            Content[Content.Count - 1] += e.Shift ? v : v.ToLower();
                        else
                            Content[Content.Count - 1] = Content[Content.Count - 1].Insert(CursorObj.X, e.Shift ? v : v.ToLower());
                        CursorObj.X++;
                        break;
                }

                TimerTinkCursor.Restart();
            }
        }

        private int Execute(string command)
        {
            command = string.Concat(command.Skip(2));

            var words = command.Split(" ").Distinct().ToList();
            string cmd = words.First();
            string subcmd = words.Skip(1).FirstOrDefault(x => !words[words.IndexOf(x) - 1].StartsWith("--") && !x.StartsWith("--"));
            Dictionary<string, string> args = new Dictionary<string, string>();
            bool valExists = false;
            foreach (string word in words)
            {
                if (valExists)
                {
                    valExists = false;
                    continue;
                }
                
                if (word.StartsWith("--") || word.StartsWith("-"))
                {
                    int dashCount = word.StartsWith("--") ? 2 : 1;
                    var key = word;
                    var idval = words.IndexOf(key) + 1;
                    valExists = idval < words.Count && !words[idval].StartsWith("--") && !words[idval].StartsWith("-");
                    var value = valExists ? words[idval] : "";
                    args[string.Concat(key.Skip(dashCount))] = value;
                }
            }

            if (cmd.CompareTo("help") == 0 || cmd.CompareTo("?") == 0)
            {
                return ShowHelp();
            }
            if (cmd.CompareTo("clear") == 0 || cmd.CompareTo("cls") == 0)
            {
                Content.Clear();
                Content.Add($"ƒ2{UserName} ƒ4MINGW64 ƒ5{Variables.LocationFormatted}");
                Content.Add($"$ ");
                CursorObj.Y = 1;
                CursorObj.X = 2;
                return -1;
            }

            if (string.IsNullOrWhiteSpace(cmd))
            {
                Content.Add("Command isn't informed.");
                return 1;
            }
            if (string.IsNullOrWhiteSpace(subcmd))
            {
                Content.Add("SubCommand isn't informed.");
                return 1;
            }

            return ExecuteCommand(cmd, subcmd, args);
        }

        private int ShowHelp()
        {
            var cmds = Variables.Commands.Where(x => Variables.Configs.InstalledPackages.Contains(x.GetType().Name));
            foreach(ICommand command in cmds)
                Content.Add($"  - {command.CommandName}  :  {command.GetType().Name}");
            return cmds.Count();
        }

        private int ExecuteCommand(string cmd, string subcmd, Dictionary<string, string> args)
        {
            var cmds = Variables.Commands.Where(x => Variables.Configs.InstalledPackages.Contains(x.GetType().Name));
            foreach (ICommand command in cmds)
            {
                if (command.CommandName.CompareTo(cmd) == 0)
                {
                    List<string> output = new List<string>();
                    if (new[] { "help", "-h" }.Contains(subcmd))
                    {
                        output = command.Help;
                    }
                    else if (command.SubCommands.ContainsKey(subcmd))
                    {
                        output = command.SubCommands[subcmd].Invoke(args);
                    }
                    Content.AddRange(output);
                    return output.Count;
                }
            }

            Content.Add("Command not found or missing package.");
            return 1;
        }
    }
}
