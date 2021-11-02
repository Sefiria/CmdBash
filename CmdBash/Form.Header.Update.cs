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
        string HeaderPath = "/";
        Rectangle btheaderXRect;
        bool IsHeaderHeld = false, IsMouseOverBtX = false, IsMouseDownBtX = false;
        Point HeaderMouseOffset = Point.Empty;

        private void HeaderUpdateInit()
        {
            string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            List<string> pathNodes = systemPath.Split(new[] { "\\" }, StringSplitOptions.None).ToList();
            pathNodes[0] = pathNodes[0].Replace(":", "").ToLower();
            pathNodes.ForEach(node => HeaderPath += node + "/");

            var sz = (int)(Header.Height - CharSize.Height) / 2;
            btheaderXRect = new Rectangle(Header.Width - sz - 16, sz, 16, 16);
        }

        private void OnHeaderMouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDownBtX = btheaderXRect.Contains(e.Location);

            if (!IsHeaderHeld)
                HeaderMouseOffset = e.Location;
            if(!IsMouseOverBtX)
                IsHeaderHeld = true;
        }
        private void OnHeaderMouseUp(object sender, MouseEventArgs e)
        {
            IsHeaderHeld = false;
            IsMouseDownBtX = false;

            if (btheaderXRect.Contains(e.Location))
            {
                Close();
                return;
            }
        }
        private void OnHeaderMouseMove(object sender, MouseEventArgs e)
        {
            IsMouseOverBtX = btheaderXRect.Contains(e.Location);

            if (!IsMouseOverBtX && IsHeaderHeld)
            {
                var x = Location.X + e.Location.X - HeaderMouseOffset.X;
                var y = Location.Y + e.Location.Y - HeaderMouseOffset.Y;
                Location = new Point(x, y);
            }
        }
    }
}
