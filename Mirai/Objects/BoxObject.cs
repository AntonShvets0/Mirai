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

        public AnimatedTextObject AnimatedTextObject;
        public CharacterObject CharacterObject;
        
        protected RectangleShape _rectangleShape;

        protected Text _characterText;
        
        public BoxObject(Text text)
        {
            AnimatedTextObject = new AnimatedTextObject(text, new Vector2f());
            _rectangleShape =
                new RectangleShape(new Vector2f(1920, 1080 * 0.27f))
                {
                    FillColor = new Color(255, 255, 255, 123)
                };

            _rectangleShape.Position = CalculatePositionBox();
            AnimatedTextObject.Position = CalculatePositionText();
            
            _characterText = new Text(AnimatedTextObject.TextTyped);
            _characterText.Position = CalculatePositionText();
            _characterText.CharacterSize += 2;
            
            _characterText.DisplayedString = "";
            _characterText.Position = new Vector2f(_characterText.Position.X, _characterText.Position.Y - 35);
        }

        public BoxObject(AnimatedTextObject animatedTextObject, RectangleShape shape)
        {
            AnimatedTextObject = animatedTextObject;
            _rectangleShape = shape;
        }

        private Vector2f CalculatePositionBox()
        {
            var y = 1080 - _rectangleShape.Size.Y;
            
            return new Vector2f(0, y);
        }

        private Vector2f CalculatePositionText()
        {
            var x = 15;
            var y = 1080 - _rectangleShape.Size.Y + 40;

            return new Vector2f(x, y);
        }

        public override IEnumerable<Drawable> Update(RenderWindow renderWindow)
        {
            _characterText.DisplayedString = CharacterObject?.Id ?? "";
            
            yield return _rectangleShape;

            foreach (var drawable in AnimatedTextObject.Update(renderWindow))
            {
                yield return Handle(drawable as Transformable);
            }

            yield return Handle(_characterText);
        }
    }
}