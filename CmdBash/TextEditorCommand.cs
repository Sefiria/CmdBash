using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CmdBash
{
    public class TextEditorCommand : ICommand
    {
        public string CommandName => "te";
        public string CurrentEditFilePath = "";

        public Dictionary<string, Func<Dictionary<string, string>, List<string>>> SubCommands => new Dictionary<string, Func<Dictionary<string, string>, List<string>>>()
        {
            ["edit"] = Edit,
            ["new"] = New,
            ["rem"] = Remove,
            ["ren"] = Rename,
        };

        public List<string> Help => new List<string> {
            "Sub commands :",
            "  - edit      Edits a file",
            "  - new      Creates a new file",
            "  - rem      Removes a file",
            "  - ren      Reanmes a file",
            "Arguments :",
            "  --name, -n :    Name of the file in current path",
            "  --new-name, -nn :    New name of the file",
        };

        private List<string> Edit(Dictionary<string, string> args)
        {
            if (Variables.Location.CompareTo("~") == 0)
                return new List<string>() { "Workspace root location not set, use 'cd' command." };

            var name = args.FirstOrDefault(x => new[] { "name", "n" }.Contains(x.Key)).Value;

            if (string.IsNullOrWhiteSpace(name))
                return new List<string>() { "Missing name, use '--name' or '-n'." };

            if (!File.Exists(Variables.Location + name))
                return new List<string>() { $"File '{name}' not found." };

            CurrentEditFilePath = Variables.Location + name;
            List<string> lines = File.ReadAllLines(CurrentEditFilePath).ToList();

            Variables.Content.Clear();
            Variables.Content.Add("$ Ctrl+S : Save, Ctrl+E : Exit");
            lines.ForEach(x => Variables.Content.Add(x));
            if (lines.Count == 0)
            {
                Variables.CursorObj.X = 0;
                Variables.CursorObj.Y = 1;
                Variables.Content.Add("");
            }
            else
            {
                Variables.CursorObj.X = Variables.Content.Last().Length;
                Variables.CursorObj.Y = Variables.Content.Count - 1;
            }
            Variables.InputHandler = InputHandler;

            return new List<string>() { Variables.NoCmdPrefixToken };
        }
        private List<string> New(Dictionary<string, string> args)
        {
            if (Variables.Location.CompareTo("~") == 0)
                return new List<string>() { "Workspace root location not set, use 'cd' command." };

            var name = args.FirstOrDefault(x => new[] { "name", "n" }.Contains(x.Key)).Value;

            if (string.IsNullOrWhiteSpace(name))
                return new List<string>() { "Missing name, use '--name' or '-n'." };

            if (File.Exists(Variables.Location + name))
                return new List<string>() { $"File '{name}' already exists." };

            File.Create(Variables.Location + name);

            return new List<string>() { $"File '{name}' created." };
        }
        private List<string> Remove(Dictionary<string, string> args)
        {
            if (Variables.Location.CompareTo("~") == 0)
                return new List<string>() { "Workspace root location not set, use 'cd' command." };

            var name = args.FirstOrDefault(x => new[] { "name", "n" }.Contains(x.Key)).Value;

            if (string.IsNullOrWhiteSpace(name))
                return new List<string>() { "Missing name, use '--name' or '-n'." };

            if (!File.Exists(Variables.Location + name))
                return new List<string>() { $"File '{name}' not found." };

            File.Delete(Variables.Location + name);

            return new List<string>() { $"File '{name}' removed." };
        }
        private List<string> Rename(Dictionary<string, string> args)
        {
            if (Variables.Location.CompareTo("~") == 0)
                return new List<string>() { "Workspace root location not set, use 'cd' command." };

            var name = args.FirstOrDefault(x => new[] { "name", "n" }.Contains(x.Key)).Value;
            var newname = args.FirstOrDefault(x => new[] { "new-name", "nn" }.Contains(x.Key)).Value;

            if (string.IsNullOrWhiteSpace(name))
                return new List<string>() { "Missing name, use '--name' or '-n'." };
            if (string.IsNullOrWhiteSpace(newname))
                return new List<string>() { "Missing new-name, use '--new-name' or '-nn'." };

            if (!File.Exists(Variables.Location + name))
                return new List<string>() { $"File '{name}' not found." };

            string content = File.ReadAllText(Variables.Location + name);
            File.Delete(Variables.Location + name);
            File.WriteAllText(Variables.Location + newname, content);

            return new List<string>() { $"File '{name}' renamed '{newname}'." };
        }

        private void InputHandler(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                File.WriteAllText(CurrentEditFilePath, string.Join(Environment.NewLine, Variables.Content.Skip(1)));
                return;
            }
            if(e.Control && e.KeyCode == Keys.E)
            {
                CurrentEditFilePath = "";
                Variables.InputHandler = Variables.DefaultInputHandler;
                Variables.ClearConsole();
                return;
            }


            if (e.KeyValue == 38 && Variables.CursorObj.Y > 1)
            {
                Variables.CursorObj.Y--;
                return;
            }
            if (e.KeyValue == 40)
            {
                Variables.CursorObj.Y++;
                if (Variables.CursorObj.Y >= Variables.Content.Count)
                    Variables.Content.Add("");
                return;
            }
            if (e.KeyValue == 37)
                {
                if (Variables.CursorObj.X > 0)
                {
                    Variables.CursorObj.X--;
                }
                else if (Variables.CursorObj.Y > 1)
                {
                    Variables.CursorObj.Y--;
                    Variables.CursorObj.X = Variables.Content[Variables.CursorObj.Y].Length;
                }
                return;
            }
            if (e.KeyValue == 39)
            {
                if (Variables.CursorObj.X < Variables.MaxContentLength - 1)
                {
                    Variables.CursorObj.X++;
                }
                else if (Variables.CursorObj.Y < Variables.Content.Count - 1)
                {
                    Variables.CursorObj.Y++;
                    Variables.CursorObj.X = 0;
                }
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
                    case 54: if (!e.Shift) v = "-"; break;
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
                        if (Variables.CursorObj.Y == 0 || (Variables.CursorObj.Y < 2 && Variables.CursorObj.X == 0))
                            break;
                        if (Variables.CursorObj.X > 0)
                        {
                            if (Variables.CursorObj.X > Variables.Content[Variables.CursorObj.Y].Length)
                            {
                                Variables.CursorObj.X--;
                            }
                            else
                            {
                                Variables.Content[Variables.CursorObj.Y] = Variables.Content[Variables.CursorObj.Y].Remove(Variables.CursorObj.X - 1, 1);
                                Variables.CursorObj.X--;
                            }
                        }
                        else
                        {
                            var length = Variables.Content[Variables.CursorObj.Y].Length;
                            if (length > 0)
                                Variables.Content[Variables.CursorObj.Y - 1] += Variables.Content[Variables.CursorObj.Y];
                            Variables.Content.RemoveAt(Variables.CursorObj.Y);
                            Variables.CursorObj.Y--;
                            Variables.CursorObj.X = Variables.Content[Variables.CursorObj.Y].Length - length;
                        }
                        break;

                    case 13:
                        if (Variables.CursorObj.X > Variables.Content[Variables.CursorObj.Y].Length)
                            Variables.Content[Variables.CursorObj.Y] += string.Concat(Enumerable.Repeat(' ', Variables.Content[Variables.CursorObj.Y].Length - Variables.CursorObj.X));
                        Variables.Content.Insert(Variables.CursorObj.Y + 1, string.Concat(Variables.Content[Variables.CursorObj.Y].Skip(Variables.CursorObj.X)));
                        if (Variables.Content[Variables.CursorObj.Y].Skip(Variables.CursorObj.X).Count() > 0)
                            Variables.Content[Variables.CursorObj.Y] = string.Concat(Variables.Content[Variables.CursorObj.Y].Skip(Variables.CursorObj.X));
                        Variables.CursorObj.X = 0;
                        Variables.CursorObj.Y++;
                        break;

                    default:
                        if (Variables.CursorObj.X > Variables.Content[Variables.CursorObj.Y].Length)
                            Variables.Content[Variables.CursorObj.Y] += string.Concat(Enumerable.Repeat(' ', Variables.CursorObj.X - Variables.Content[Variables.CursorObj.Y].Length));
                        if (Variables.CursorObj.X == Variables.Content[Variables.Content.Count - 1].Length)
                            Variables.Content[Variables.CursorObj.Y] += e.Shift ? v : v.ToLower();
                        else
                            Variables.Content[Variables.CursorObj.Y] = Variables.Content[Variables.CursorObj.Y].Insert(Variables.CursorObj.X, e.Shift ? v : v.ToLower());
                        Variables.CursorObj.X++;
                        break;
                }
            }
        }
    }
}
