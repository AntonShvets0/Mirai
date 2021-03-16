using System.Collections.Generic;

namespace Mirai.Novell.Objects
{
    public class TextData
    {
        public Character Speaker;
        public string[] Text;
        public int IndexText = 0;
        
        public List<Character> Characters = new List<Character>();

        public TextData(Character character, string text)
        {
            Speaker = character;
            Characters.Add(Speaker);
            Text = new []
            {
                T._(text)
            };
        }

        public TextData(Character character, string[] text)
        {
            Speaker = character;
            Characters.Add(Speaker);

            for (int i = 0; i < text.Length; i++)
            {
                text[i] = T._(text[i]);
            }
            
            Text = text;
        }
    }
}