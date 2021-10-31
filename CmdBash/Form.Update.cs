using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CmdBash
{
    public partial class Form : System.Windows.Forms.Form
    {
        char TokenFormat = 'ƒ';
        List<string> Content = new List<string>();
        string Path = "/";
        Rectangle btXRect;

        private void UpdateInit()
        {
            string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            List<string> pathNodes = systemPath.Split(new[] { "\\" }, StringSplitOptions.None).ToList();
            pathNodes[0] = pathNodes[0].Replace(":", "").ToLower();
            pathNodes.ForEach(node => Path += node + "/");

            var sz = (Header.Height - CharSize.Height) / 2;
            btXRect = new Rectangle(Header.Width - sz - 16, sz, 16, 16);
        }

        private void Update(object sender, EventArgs e)
        {
            List<string> content = new List<string>(Content);
            Content.Clear();
            foreach (string line in content)
            {
                Content.AddRange(line.Split(new[] { CR }, StringSplitOptions.None));
            }
        }


        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (btXRect.Contains(e.Location))
            {
                Close();
            }
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
    }
}
