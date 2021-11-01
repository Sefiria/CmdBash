using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace CmdBash
{
    public class Tools
    {

        public static Random rnd = new Random((int)DateTime.Now.Ticks);
        
        static public float Lerp(float firstFloat, float secondFloat, float by)
        {
            return (1F - by) * firstFloat + by * secondFloat;
        }
        static public Bitmap DecodeBitmap(string bytes)
        {
            string[] arr = bytes.Split('-');
            byte[] array = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            return (Bitmap)tc.ConvertFrom(array);
        }
        static public bool CompareColors(Color a, Color b)
        {
            return a.ToArgb() == b.ToArgb();
        }
        static public string GetBitmapEncoded(Bitmap bmp)
        {
            return BitConverter.ToString((byte[])new ImageConverter().ConvertTo(bmp, typeof(byte[])));
        }
        static public List<Bitmap> SplitBitmap(Bitmap img, int size)
        {
            List<Bitmap> result = new List<Bitmap>();
            for (int y = 0; y < img.Size.Height / size; y++)
            {
                for (int x = 0; x < img.Size.Width / size; x++)
                {
                    var splitted = new Bitmap(size, size);
                    using (var g = Graphics.FromImage(splitted))
                        g.DrawImage(img, new Rectangle(0, 0, size, size), new Rectangle(x * size, y * size, size, size), GraphicsUnit.Pixel);
                    splitted.MakeTransparent();
                    result.Add(splitted);
                }
            }
            return result;
        }

        [DllImport("shlwapi.dll")]
        public static extern int ColorHLSToRGB(int H, int L, int S);

        // Convert an object to a byte array
        static public byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        static public object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }

        public static void Serialize(object obj, string filename)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamWriter(filename))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
            }
        }
        static public void SerializeJSON(object data, string filename)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
        public static object Deserialize(string path)
        {
            var serializer = new JsonSerializer();
            object result = null;

            using (var sw = new StreamReader(path))
            using (var reader = new JsonTextReader(sw))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }
        static public T DeserializeJSON<T>(string JSON) where T:class
        {
            return JsonConvert.DeserializeObject<T>(JSON);
        }
        static public T DeserializeJSONFromFile<T>(string path) where T : class
        {
            return DeserializeJSON<T>(File.ReadAllText(path));
        }

        public static T EnumParser<T>(string name) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return default(T);

            return (T)Enum.Parse(typeof(T), name);
        }
        
        public static int GetFilesCount(string ResourcesPath, string extension)
        {
            return Directory.GetFiles(ResourcesPath, extension, SearchOption.TopDirectoryOnly).Length;
        }
        public static bool IsVisible(Graphics g, Point pt, int W, int H)
        {
            if (g.VisibleClipBounds.Contains(pt.X, pt.Y))
                return true;
            if (g.VisibleClipBounds.Contains(pt.X + W - 1, pt.Y))
                return true;
            if (g.VisibleClipBounds.Contains(pt.X, pt.Y + H - 1))
                return true;
            if (g.VisibleClipBounds.Contains(pt.X + W - 1, pt.Y + H - 1))
                return true;

            return false;
        }
    }
}
