using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculs
{
    public partial class Form1 : Form
    {
        public class Bullet
        {
            public Point Loc;
            public PointF Look;
            public float Fall, DefaultFall = 1F;
            public int TimeLife, DefaultTimeLife = 100;
            public Bullet(Point Loc, PointF Look)
            {
                this.Loc = Loc;
                this.Look = Look;
                Fall = DefaultFall;
                TimeLife = DefaultTimeLife;
            }
        }

        Graphics g, gd;
        Bitmap img;
        Bitmap imgDraw;
        Timer DrawTimer = new Timer() { Enabled = true, Interval = 10 };
        Timer UpdateTimer = new Timer() { Enabled = true, Interval = 10 };
        Random RND;
        Rectangle Pxl;
        bool jumping = false, onGround = false;
        float jumpSz = 0F, gravity = 1F;
        bool MouseHold = false;
        int speed = 2;
        Point PrevMouse = Point.Empty;
        int JumpHeight = 9;
        List<Bullet> Bullets = new List<Bullet>();

        public Form1()
        {
            InitializeComponent();

            img = new Bitmap(Render.Width, Render.Height);
            imgDraw = new Bitmap(Render.Width, Render.Height);
            g = Graphics.FromImage(img);
            gd = Graphics.FromImage(imgDraw);
            RND = new Random((int)DateTime.UtcNow.Ticks);

            Pxl = new Rectangle(160, 299, 4, 4);

            DrawTimer.Tick += Draw;
            UpdateTimer.Tick += Update;
            Render.MouseDown += OnMouseDown;
            Render.MouseUp += OnMouseUp;
            Render.MouseLeave += OnMouseLeave;
            Render.MouseMove += OnMouseMove;
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PrevMouse = e.Location;
                MouseHold = true;
            }
            else if(e.Button == MouseButtons.Left)
            {
                var look = Maths.Look(Pxl.Location, e.Location);
                var loc = new Point((int)(Pxl.Location.X + look.X * 5), (int)(Pxl.Location.Y + look.Y * 5));
                Bullets.Add(new Bullet(loc, look));
            }
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            MouseHold = false;
        }
        private void OnMouseLeave(object sender, EventArgs e)
        {
            MouseHold = false;
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!MouseHold)
                return;

            var step = 1 / (float)Math.Max(Math.Max(Math.Abs(e.X), Math.Abs(PrevMouse.X)), Math.Max(Math.Abs(e.Y), Math.Abs(PrevMouse.Y)));
            for (float t = 0F; t <= 1F; t += step)
            {
                var pt = e.Location.Lerp2(PrevMouse, t);
                gd.DrawRectangle(Pens.White, pt.X, pt.Y, 1, 1);
            }
            PrevMouse = e.Location;
        }

        private void Update(object sender, EventArgs e)
        {
            onGround = Pxl.CollisionLerp(img, 0, (int)gravity);
                
            if (InputsSimulation.GetKeyState(InputsSimulation.VirtualKeyShort.KEY_Q) < 0)
                if (!Pxl.CollisionLerp(img, -speed, 0))
                    Pxl.X-=4;
            else if (!new Rectangle(Pxl.X - speed, Pxl.Y, Pxl.Width, Pxl.Height).CollisionLerp(img, 0, -speed))
                {
                    Pxl.X -= speed;
                    Pxl.Y -= speed;
                }
            if (InputsSimulation.GetKeyState(InputsSimulation.VirtualKeyShort.KEY_D) < 0)
                if (!Pxl.CollisionLerp(img, speed, 0))
                    Pxl.X += speed;
                else if (!new Rectangle(Pxl.X + speed, Pxl.Y, Pxl.Width, Pxl.Height).CollisionLerp(img, 0, -speed))
                {
                    Pxl.X += speed;
                    Pxl.Y -= speed;
                }

            if (InputsSimulation.GetKeyState(InputsSimulation.VirtualKeyShort.SPACE) < 0 && onGround && !jumping)
            {
                jumping = true;
                jumpSz = 0;
            }

            if (jumping && !Pxl.CollisionLerp(img, 0, - JumpHeight - (int)jumpSz))
            {
                Pxl.Y -= JumpHeight - (int)jumpSz;
                jumpSz += 1F;
            }
            else
            {
                jumping = false;
            }
            if(jumpSz >= JumpHeight)
            {
                jumping = false;
            }

            if (!onGround)
            {
                Pxl.Y += (int)gravity;
                gravity += 0.1F;
            }
            else
                gravity = 1F;





            var bullets = new List<Bullet>(Bullets);
            foreach (Bullet b in bullets)
            {
                if (!b.Loc.CollisionLerp(img, new Point((int)(b.Look.X * 10), (int)(b.Look.Y * 10)), new List<Color>() { Color.White, Color.Red, Color.Lime, Color.Blue }))
                {
                    b.Loc.X += (int)(b.Look.X * 10);
                    b.Loc.Y += (int)(b.Look.Y * 10);
                }
                else
                {
                    b.Look.X = b.Look.Y = 0F;
                }
                if(!b.Loc.CollisionLerp(img, 0, (int)b.Fall + 1))
                {
                    b.Loc.Y += (int)b.Fall;
                    b.Fall += 0.2F;
                    b.TimeLife = b.DefaultTimeLife;
                }
                else
                {
                    b.Fall = 1F;
                    b.Look.X = b.Look.Y = 0F;
                    if (b.TimeLife > 0)
                        b.TimeLife--;
                    else
                        Bullets.Remove(b);
                }
            }
        }
        private void Draw(object sender, EventArgs e)
        {
            g.Clear(Color.Black);

            g.DrawRectangle(Pens.White, 20, 20, 300, 300);

            g.DrawImage(imgDraw, 0, 0);

            foreach(var b in Bullets)
                g.FillRectangle(Brushes.Red, b.Loc.X, b.Loc.Y, 2, 2);

            g.FillRectangle(Brushes.Lime, Pxl);

            Render.Image = img;
        }
    }
}
