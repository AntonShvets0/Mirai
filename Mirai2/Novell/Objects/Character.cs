using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace Mirai.Novell.Objects
{
    public class Character
    {
        public string Name;
        
        public Color Color;
        public string Texture;
        public Vector2f Position;

        public int Emotion = (int)Enums.Emotion.Normal;
        
        private static List<Character> _characters = new List<Character>();
        public static Character Find(string name)
        {
            name = T._(name);
            return _characters.FirstOrDefault(c => c.Name == name);
        }

        public static Character Add(Character character)
        {
            _characters.Add(character);
            return character;
        }

        public Character(string name, string texture)
        {
            Name = T._(name);
            Texture = texture;
            Color = Color.White;
        }
    
        public Character(string name, Color color, string texture)
        {
            Name = T._(name);
            Texture = texture;
            Color = color;
        }
    }
}