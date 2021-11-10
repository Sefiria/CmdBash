using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculs
{
    public static class ExtensionMethods
    {
        public static bool Collision(this Point pt, Bitmap img, int toX, int toY)
        {
            if (pt.X + toX < 0 || pt.X + toX >= img.Width || pt.Y + toY < 0 || pt.Y + toY >= img.Height)
                return true;
            return img.GetPixel(pt.X + toX, pt.Y + toY).ToArgb() == Color.White.ToArgb();
        }
        public static bool Collision(this Rectangle r, Bitmap img, int toX, int toY)
        {
            if (toY == -1)
            {
                for (int x = r.X; x < r.X + r.Width; x++)
                    if (new Point(x, r.Y).Collision(img, toX, toY))
                        return true;
            }

            if (toY == 1)
            {
                for (int x = r.X; x < r.X + r.Width; x++)
                    if (new Point(x, r.Y + r.Height - 1).Collision(img, toX, toY))
                        return true;
            }

            if (toX == -1)
            {
                for (int y = r.Y; y < r.Y + r.Height; y++)
                    if (new Point(r.X, y).Collision(img, toX, toY))
                        return true;
            }

            if (toX == 1)
            {
                for (int y = r.Y; y < r.Y + r.Height; y++)
                    if (new Point(r.X + r.Width - 1, y).Collision(img, toX, toY))
                        return true;
            }

            return false;
        }
        public static bool CollisionLerp(this Point pt, Bitmap img, int toX, int toY)
        {
            for (float t = 0F; t <= 1F; t += 1 / (float)Math.Max(Math.Abs(toX), Math.Abs(toY)))
            {
                var x = (int)pt.X.Lerp(pt.X + toX, t);
                var y = (int)pt.Y.Lerp(pt.Y + toY, t);
                if (x < 0 || x >= img.Width || y < 0 || y >= img.Height)
                    return true;
                if (img.GetPixel(x, y).ToArgb() == Color.White.ToArgb())
                    return true;
            }
            return false;
        }
        public static bool CollisionLerp(this Rectangle r, Bitmap img, int toX, int toY)
        {
            if (toY < 0)
            {
                for (int x = r.X; x < r.X + r.Width; x++)
                    if (new Point(x, r.Y).CollisionLerp(img, toX, toY))
                        return true;
            }

            if (toY > 0)
            {
                for (int x = r.X; x < r.X + r.Width; x++)
                    if (new Point(x, r.Y + r.Height - 1).CollisionLerp(img, toX, toY))
                        return true;
            }

            if (toX < 0)
            {
                for (int y = r.Y; y < r.Y + r.Height; y++)
                    if (new Point(r.X, y).CollisionLerp(img, toX, toY))
                        return true;
            }

            if (toX > 0)
            {
                for (int y = r.Y; y < r.Y + r.Height; y++)
                    if (new Point(r.X + r.Width - 1, y).CollisionLerp(img, toX, toY))
                        return true;
            }

            return false;
        }
        public static bool CollisionLerp(this Point pt, Bitmap img, Point look, List<Color> colors)
        {
            for (float t = 0F; t <= 1F; t += 1 / (float)Math.Max(Math.Abs(look.X), Math.Abs(look.Y)))
            {
                var x = (int)pt.X.Lerp(pt.X + look.X, t);
                var y = (int)pt.Y.Lerp(pt.Y + look.Y, t);
                if (x == pt.X && y == pt.Y)
                    continue;
                if (x < 0 || x >= img.Width || y < 0 || y >= img.Height)
                    return true;
                if (colors.Select(c => c.ToArgb()).Contains(img.GetPixel(x, y).ToArgb()))
                    return true;
            }
            return false;
        }

        public static float Lerp(this int v0, float v1, float t)
        {
            return (1F - t) * v0 + t * v1;
        }
        public static Point Lerp2(this Point pt0, Point pt1, float t)
        {
            return new Point((int)Lerp(pt0.X, pt1.X, t), (int)Lerp(pt0.Y, pt1.Y, t));
        }
    }
}
