using System.IO;
using System.IO.Compression;

namespace Mirai.ByteSaver
{
    public class SaveManager
    {
        public static void SaveToFile(string file, object obj) 
            => File.WriteAllBytes(file, Compress(new Serializer(obj).ToBytes()));

        public static T GetFromFile<T>(string file) where T : class
            => new Deserializer<T>(Decompress(File.ReadAllBytes(file))).Deserialize();

        private static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        private static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}