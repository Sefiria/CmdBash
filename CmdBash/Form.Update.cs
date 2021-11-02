using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

namespace CmdBash
{
    public partial class Form : System.Windows.Forms.Form
    {
        char TokenFormat = 'ƒ';
        List<string> Content = new List<string>();
        List<string> History = new List<string>();
        int HistoryCur = -1;
        CursorObj CursorObj;
        Stopwatch TimerTinkCursor = new Stopwatch();
        List<ICommand> Commands;


        private void UpdateInit()
        {
            HeaderUpdateInit();

            var type = typeof(ICommand);
            Commands = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface)
                .Select(x => (ICommand)Activator.CreateInstance(x))
                .ToList();

            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Content.Add($"ƒ2{userName} ƒ4MINGW64 ƒ5~");
            Content.Add($"$ ");

            CursorObj = new CursorObj(Content[Content.Count - 1].Length, Content.Count - 1);
            TimerTinkCursor.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            List<string> content = new List<string>(Content);
            Content.Clear();
            foreach (string line in content)
            {
                Content.AddRange(line.Split(new[] { CR }, StringSplitOptions.None));
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
            Console.WriteLine(e.KeyValue);

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


            if ((e.KeyValue >= 65 && e.KeyValue <= 90)
                || (e.KeyValue >= 96 && e.KeyValue <= 105)
                || (e.Shift && e.KeyValue >= 48 && e.KeyValue <= 57)
                || new List<int> { 8, 13, 32, 54, 106, 107, 109, 111, 187, 220 }.Contains(e.KeyValue))
            {
                string v = $"{(char)e.KeyValue}";
                if (e.KeyValue >= 96 && e.KeyValue <= 105)
                    v = $"{e.KeyValue - 96}";
                if (e.Shift && e.KeyValue >= 48 && e.KeyValue <= 57)
                    v = $"{e.KeyValue - 48}";

                switch (e.KeyValue)
                {
                    case 111: v = "/"; break;
                    case 106: v = "*"; break;
                    case 109: v = "-"; break;
                    case 54: if(!e.Shift) v = "-"; break;
                    case 107: v = "+"; break;
                    case 187: v = e.Shift ? "+" : "="; break;
                    case 220: v = "*"; break;
                }
                
                switch(e.KeyValue)
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
                            int nblines = Execute(Content[Content.Count - 1]) + 1;
                            Content.Add("$ ");
                            CursorObjNextLine(nblines);
                        }
                        break;

                    default:
                        Content[Content.Count - 1] += e.Shift ? v : v.ToLower();
                        CursorObj.X++;
                        break;
                }

                TimerTinkCursor.Restart();
            }
        }

        private int Execute(string command)
        {
            command = string.Concat(command.Skip(2));

            var words = command.Split(" ").ToList();
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
                
                if (word.StartsWith("--"))
                {
                    var key = word;
                    var idval = words.IndexOf(key) + 1;
                    valExists = idval < words.Count && !words[idval].StartsWith("--");
                    var value = valExists ? words[idval] : "";
                    args[string.Concat(key.Skip(2))] = value;
                }
            }

            if (cmd.CompareTo("help") == 0 || cmd.CompareTo("?") == 0)
            {
                return ShowHelp();
            }

            if (string.IsNullOrWhiteSpace(cmd)) return 0;
            if (string.IsNullOrWhiteSpace(subcmd)) return 0;

            return ExecuteCommand(cmd, subcmd, args);
        }

        private int ShowHelp()
        {
            foreach(ICommand command in Commands)
                Content.Add($"\t- {command.CommandName}");
            return Commands.Count;
        }

        private int ExecuteCommand(string cmd, string subcmd, Dictionary<string, string> args)
        {
            foreach (ICommand command in Commands)
            {
                if(command.CommandName.CompareTo(cmd) == 0 && command.SubCommands.ContainsKey(subcmd))
                {
                    List<string> output = command.SubCommands[subcmd].Invoke(args);
                    Content.AddRange(output);
                    return output.Count;
                }
            }

            return 0;
        }
    }
}
