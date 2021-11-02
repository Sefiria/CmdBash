using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CmdBash
{
    public partial class Form : System.Windows.Forms.Form
    {
        char TokenFormat = 'ƒ';
        List<string> Content = new List<string>();
        CursorObj CursorObj;
        Stopwatch TimerTinkCursor = new Stopwatch();


        private void UpdateInit()
        {
            HeaderUpdateInit();

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
            bool execute = false;
            //Console.WriteLine(e.KeyValue);

            if ((e.KeyValue >= 65 && e.KeyValue <= 90)
                || (e.KeyValue >= 96 && e.KeyValue <= 105)
                || new List<int> { 8, 13, 32, 54, 106, 107, 109, 111, 187, 220 }.Contains(e.KeyValue))
            {
                string v = $"{(char)e.KeyValue}";
                if (e.KeyValue >= 96 && e.KeyValue <= 105)
                    v = $"{e.KeyValue - 96}";

                switch (e.KeyValue)
                {
                    case 111: v = "/"; break;
                    case 106: v = "*"; break;
                    case 109: case 54: v = "-"; break;
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
                            Content.Add("$ ");
                            CursorObj.X = 2;
                            CursorObj.Y++;
                            execute = true;
                        }
                        break;

                    default:
                        Content[Content.Count - 1] += e.Shift ? v : v.ToLower();
                        CursorObj.X++;
                        break;
                }

                TimerTinkCursor.Restart();

                if(execute)
                {
                    Execute(Content[Content.Count - 2]);
                }
            }
        }

        private void Execute(string command)
        {
            command = string.Concat(command.Skip(2));

            var words = command.Split(" ");
            string cmd = words.First();
            string subcmd = words.Skip(1).Where(x => !x.StartsWith("--")).FirstOrDefault();
            Dictionary<string, string> args = new Dictionary<string, string>();

        }
    }
}
