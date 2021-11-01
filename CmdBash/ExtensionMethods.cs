using System;
using System.Drawing;

namespace CmdBash
{
    public static class ExtensionMethods
    {
        public static Rectangle GetInflated(this Rectangle target, int size) => GetInflated(target, size, size);
        public static Rectangle GetInflated(this Rectangle target, int width, int height)
        {
            var result = target;
            result.Inflate(width, height);
            return result;
        }
        public static string[] Split(this string text, string separator) => text.Split(new string[] { separator }, StringSplitOptions.None);
    }
}
