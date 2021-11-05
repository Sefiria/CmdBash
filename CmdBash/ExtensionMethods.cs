using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static string[] Split(this string text, string separator, bool removeEmpty = false) => text.Split(new string[] { separator }, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> result, List<T> list) => result.AddRange(list.ToArray());
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> result, T[] list)
        {
            foreach (T element in list)
            {
                result.Add(element);
            }
            return result;
        }
    }
}
