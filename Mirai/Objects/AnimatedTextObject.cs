using System.Collections.Generic;
using Mirai.Abstracts;
using Mirai.Animation;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai.Objects
{
    public class AnimatedTextObject : GameObject
    {
        public override Cursor FocusedCursor { get; set; }
        public override bool IsFocusable { get; set; } = false;

        public string Text;
        public Text TextTyped;
        public Vector2f Position;
        private int _frame;

        public AnimatedTextObject(Text text, Vector2f position)
        {
            Position = position;
            Text = "";
            text.DisplayedString = "";
            
            TextTyped = text;
        }
        
        public override IEnumerable<Drawable> Update(RenderWindow renderWindow)
        { 
            TextTyped.Position = Position;
            
            if (TextTyped.DisplayedString == Text)
            {
                yield return TextTyped;
                yield break;
            }

            _frame++;
            if (_frame > 10)
            {
                _frame = 0;
                TextTyped.DisplayedString += Text.Substring(TextTyped.DisplayedString.Length, 1);
            }

            yield return TextTyped;
        }
    }
}