using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculs
{
    public class Maths
    {
        public static float Lerp(int v0, float v1, float t)
        {
            return (1F - t) * v0 + t * v1;
        }
        public static Point Lerp2(Point pt0, Point pt1, float t)
        {
            return new Point((int)Lerp(pt0.X, pt1.X, t), (int)Lerp(pt0.Y, pt1.Y, t));
        }
        public static PointF Look(Point source, Point target)
        {
            float distance = (float)Math.Sqrt((target.X - source.X) * (target.X - source.X) + (target.Y - source.Y) * (target.Y - source.Y));
            return new PointF((target.X - source.X) / distance, (target.Y - source.Y) / distance);
        }
    }
}
