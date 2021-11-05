using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CmdBash
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap img, img_header;
        Graphics g, g_header;
        new Font Font;
        SizeF CharSize = new SizeF(7.45F, 15);
        int Offset = 4;
        string CR = Environment.NewLine;
        int LogoSize = 16;
        float MaxLineLength;
        int ScrollValue = 0;

        private void DrawInit()
        {
            img = new Bitmap(Render.Width, Render.Height);
            img_header = new Bitmap(Header.Width, Header.Height);
            g = Graphics.FromImage(img);
            g_header = Graphics.FromImage(img_header);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g_header.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            Font = new Font("Lucida Console", 9F, FontStyle.Regular);
            MaxLineLength = (480 - Offset * 2) / CharSize.Width;
        }

        private void Draw(object sender, EventArgs e)
        {
            g.Clear(Color.Black);

            DrawHeader();

            List<string> treat = new List<string>();
            List<string> lines = new List<string>();
            var c = Content.Skip(ScrollValue);
            foreach (string line in c)
            {
                string l = line;
                treat.Clear();
                while(GetUnformattedLine(l).Length > MaxLineLength)
                {
                    treat.Add(string.Concat(l.Take((int)MaxLineLength)));
                    l = string.Concat(l.Skip((int)MaxLineLength));
                }
                treat.Add(l);
                lines.AddRange(treat);
            }

            SolidBrush b = new SolidBrush(Color.White);
            float x;
            for(int l=0; l<lines.Count; l++)
            {
                string line = lines[l];

                string[] words = Regex.Split(line, $@"(?<={TokenFormat}\d| )");

                x = 0F;
                b.Color = Color.White;
                foreach(string word in words)
                {
                    if (word.Length == 0)
                        continue;
                    if (word[0] == TokenFormat)
                    {
                        b.Color = GetColorFromFormat(word[1]);
                    }
                    else
                    {
                        if (word.StartsWith("--"))
                        {
                            b.Color = GetColor(7);
                        }

                        g.DrawString(word, Font, b, Offset + x, Offset + l * CharSize.Height);
                        x += CharSize.Width * word.Length;
                        b.Color = Color.White;
                    }
                }
            }

            if (TimerTinkCursor.ElapsedMilliseconds < 500)
            {
                g.DrawLine(Pens.White,
                            Offset + CursorObj.X * CharSize.Width + 1,
                            Offset / 2 + (CursorObj.Y - ScrollValue) * CharSize.Height,
                            Offset + CursorObj.X * CharSize.Width + 1,
                            ((CursorObj.Y - ScrollValue) + 1) * CharSize.Height);
            }

            Pen pen = new Pen(Color.FromArgb(80, 80, 80), 2F);
            g.DrawLine(pen, 0, 0, 0, Render.Height);
            g.DrawLine(pen, 0, Render.Height, Render.Width, Render.Height);
            g.DrawLine(pen, Render.Width, 0, Render.Width, Render.Height);

            Render.Image = img;
        }

        private void DrawHeader()
        {
            var hhh = (Header.Height - CharSize.Height) / 2F;
            g_header.DrawEllipse(Pens.Red, hhh, hhh, LogoSize, LogoSize);
            g_header.DrawString($"MINGW64:{HeaderPath}", new Font(Font.FontFamily, 10F), Brushes.White, hhh * 2 + LogoSize, hhh + 2);

            Pen pen = new Pen(Color.FromArgb(80, 80, 80), 2F);
            g_header.DrawLine(pen, 0, 0, 0, Render.Height);
            g_header.DrawLine(pen, 0, 0, Render.Width, 0);
            g_header.DrawLine(pen, Render.Width, 0, Render.Width, Render.Height);

            g_header.FillRectangle(new SolidBrush(IsMouseDownBtX ? Color.Yellow : (IsMouseOverBtX ? Color.LightGray : Color.Gray)), btheaderXRect);
            g_header.DrawRectangle(Pens.Red, btheaderXRect);
            g_header.DrawLine(Pens.Red, btheaderXRect.X, btheaderXRect.Y, btheaderXRect.X + btheaderXRect.Width, btheaderXRect.Y + btheaderXRect.Height);
            g_header.DrawLine(Pens.Red, btheaderXRect.X + btheaderXRect.Width, btheaderXRect.Y, btheaderXRect.X, btheaderXRect.Y + btheaderXRect.Height);

            Header.Image = img_header;
        }

        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {
            ScrollValue += - e.Delta / 100;
            var max = Content.Count + 1;
            if (ScrollValue < 0)
                ScrollValue = 0;
            if (ScrollValue > max)
                ScrollValue = max;
        }
    }
}
