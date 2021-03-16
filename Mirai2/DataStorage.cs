using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Audio;
using SFML.Graphics;

namespace Mirai
{
    public class DataStorage
    {
        public Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
        public Dictionary<string, SoundBuffer> Sounds = new Dictionary<string, SoundBuffer>();
        public Dictionary<string, Font> Fonts = new Dictionary<string, Font>();

        public delegate void LoadEvent(string fileName, object data);

        public event LoadEvent OnTextureLoaded;
        public event LoadEvent OnSoundLoaded;
        public event LoadEvent OnFontLoaded;
        

        public Texture GetTexture(string key) => Textures[key];
        public Font GetFont(string key) => Fonts[key];

        public void LoadTextures()
        {
            foreach (string file in Directory.EnumerateFiles("assets/textures", "*.*", SearchOption.AllDirectories))
            {
                var texture = new Texture(file);
                
                Textures.Add(file.Substring("assets/textures".Length + 1).Replace("\\", "/"), texture);
                OnTextureLoaded?.Invoke(file, texture);
            }
        }

        public void LoadFonts()
        {
            foreach (string file in Directory.EnumerateFiles("assets/fonts", "*.*", SearchOption.AllDirectories))
            {
                var font = new Font(file);
                
                Fonts.Add(file.Substring("assets/fonts".Length + 1).Replace("\\", "/"), font);
                OnFontLoaded?.Invoke(file, font);
            }
        }

        public void LoadSounds()
        {
            foreach (string file in Directory.EnumerateFiles("assets/music", "*.*", SearchOption.AllDirectories))
            {
                var sound = new SoundBuffer(file);
                
                Sounds.Add(file.Substring("assets/music".Length + 1).Replace("\\", "/"), sound);
                OnSoundLoaded?.Invoke(file, sound);
            }
        }
    }
}