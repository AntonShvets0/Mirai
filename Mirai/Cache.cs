using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Audio;
using SFML.Graphics;

namespace Mirai
{
    public class Cache
    {
        private static Dictionary<string, object> _cache = new Dictionary<string, object>();
        public static string LanguageCode;
        
        public static void Load()
        {
            _cache.Clear();
            foreach (string file in Directory.EnumerateFiles("assets/", "*.*", SearchOption.AllDirectories))
            {
                if (file.StartsWith("assets/localization/"))
                {
                    if (!file.StartsWith("assets/localization/" + LanguageCode)) continue;
                }
                
                var response = LoadFile(file);
                _cache.Add(file.Substring("assets/".Length).Replace("\\", "/"), response);
            }
        }

        public static object Get(string file) => _cache.ContainsKey(file) ? _cache[file] : null;
        public static Font GetFont(string file) => (Font) Get(file.Contains(".") ? file : file + ".ttf");
        public static Texture GetTexture(string file) => (Texture) Get(file.Contains(".") ? file : file + ".png");
        public static SoundBuffer GetSound(string file) => (SoundBuffer) Get(file.Contains(".") ? file : file + ".mp4");

        private static object LoadFile(string file)
        {
            if (file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".png") || file.EndsWith(".gif"))
                return new Texture(new StreamReader(file).BaseStream);

            if (file.EndsWith(".ttf"))
                return new Font(new StreamReader(file).BaseStream);

            if (file.EndsWith(".wav") || file.EndsWith(".mp4"))
                return new SoundBuffer(new StreamReader(file).BaseStream);
                
            return null;
        }
    }
}