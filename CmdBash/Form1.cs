using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CmdBash
{
    public partial class Form : System.Windows.Forms.Form
    {
        Timer TimerDraw = new Timer() { Interval = 10, Enabled = true };
        Timer TimerUpdate = new Timer() { Interval = 10, Enabled = true };

        public Form()
        {
            InitializeComponent();

            Width = 480;
            Height = 520;

            UpdateInit();
            DrawInit();

            TimerUpdate.Tick += Update;
            TimerDraw.Tick += Draw;
            MouseDown += OnMouseDown;
        }
    }
}
