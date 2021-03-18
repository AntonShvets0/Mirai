using System.Collections.Generic;
using Mirai.Abstracts;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai.Objects
{
    public class BoxObject : GameObject
    {
        public override bool IsFocusable { get; set; } = false;
        public override Cursor FocusedCursor { get; set; }
        
        public RectangleShape RectangleShape;
        public string UpperText;
        protected Text _characterText;
        
        public BoxObject(Text text)
        {
            RectangleShape =
                new RectangleShape(new Vector2f(1920, 1080 * 0.27f))
                {
                    FillColor = new Color(255, 255, 255, 123)
                };

            RectangleShape.Position = new Vector2f(0, 1080 - RectangleShape.Size.Y);
            
            _characterText = new Text(text);
            _characterText.Position = new Vector2f(15, 1080 - RectangleShape.Size.Y + 40);
            _characterText.CharacterSize += 2;
            
            _characterText.DisplayedString = "";
            _characterText.Position = new Vector2f(_characterText.Position.X, _characterText.Position.Y - 35);
        }

        public override IEnumerable<Drawable> Update(RenderWindow renderWindow)
        {
            _characterText.DisplayedString = UpperText ?? "";
            
            yield return RectangleShape;
            yield return Handle(_characterText);
        }
    }
}