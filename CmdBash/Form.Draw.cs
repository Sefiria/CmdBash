using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CmdBash
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap img, img_header;
        Graphics g, g_header;
        new Font Font;
        Size CharSize = new Size(7, 15);
        int Offset = 4;
        string CR = Environment.NewLine;
        int LogoSize = 16;
        int MaxLineLength;

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
            foreach(string line in Content)
            {
                string l = line;
                treat.Clear();
                while(GetUnformattedLine(l).Length > MaxLineLength)
                {
                    treat.Add(string.Concat(l.Take(MaxLineLength)));
                    l = string.Concat(l.Skip(MaxLineLength));
                }
                treat.Add(l);
                lines.AddRange(treat);
            }

            SolidBrush b = new SolidBrush(Color.White);
            int x;
            for(int l=0; l<lines.Count; l++)
            {
                string line = lines[l];
                b.Color = Color.White;
                int charCount = line.Length;
                x = 0;
                for(int c=0; c<charCount; c++)
                {
                    char @char = line[c];

                    if (@char != TokenFormat)
                    {
                        Size sz = TextRenderer.MeasureText(g, "i", Font);
                        g.DrawString($"{@char}", Font, b, Offset + x, Offset + l * CharSize.Height);
                        x += CharSize.Width;
                    }
                    else
                    {
                        b.Color = GetColorFromFormat(line[++c]);
                    }
                }
            }

            if (TimerTinkCursor.ElapsedMilliseconds < 500)
            {
                g.DrawLine(Pens.White,
                            CursorObj.X * CharSize.Width,
                            CursorObj.Y * CharSize.Height,
                            CursorObj.X * CharSize.Width,
                            (CursorObj.Y + 1) * CharSize.Height - 1);
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
    }
}
