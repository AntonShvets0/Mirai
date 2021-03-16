using System.Collections.Generic;
using Mirai.Abstracts;
using Mirai.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai.Objects
{
    public class MessageBoxObject : GameObject
    {
        public override bool IsFocusable { get; set; } = true;
        public override Cursor FocusedCursor { get; set; } = new Cursor(Cursor.CursorType.Hand);

        private RectangleShape _shape;
        public Text Message;
        public Style Style;
        private float _padding = 30f;

        private Vector2f _position;
        private bool _isPositionSet = false;
        
        public MessageBoxObject(Style style, Text message)
        {
            Style = style;
            Message = message;
        }

        public override IEnumerable<Drawable> Update(RenderWindow renderWindow)
        {
            var size = CalculateSize(renderWindow);

            if (!_isPositionSet)
            {
                _position = new Vector2f(1920 / 2f - size.X, 1080 / 2f - size.Y);
                _isPositionSet = true;
            }
            
            _shape = new RectangleShape(size)
            {
                FillColor = Style.GetColor(StyleType.Color) ?? Color.White,
                OutlineColor = Style.GetColor(StyleType.BorderColor) ?? Color.Black,
                OutlineThickness = Style.GetIntNullable(StyleType.BorderSize) ?? 1,
                Position = _position
            };
            
            yield return Handle(_shape);

            yield return new Text(Message)
            {
                DisplayedString = "Message Box",
                Position = new Vector2f(_position.X + 10, _position.Y + 10),
                FillColor = Color.Blue
            };

            Message.Position = CalculateTextPosition();
            Message.FillColor = Style.GetColor(StyleType.TextColor) ?? Color.Black;

            yield return Handle(Message);
        }
        
        private Vector2f CalculateTextPosition()
        {
            return new Vector2f(_position.X + _padding, _position.Y + _padding);
        }

        private Vector2f CalculateSize(RenderWindow renderWindow)
        {
            float width;
            float height;

            var bounds = Message.GetLocalBounds();
            if (Style.GetFloatNullable(StyleType.Width) == null)
                width = bounds.Width + _padding * 4;
            else
                width = Style.GetFloat(StyleType.Width) + _padding * 4;

            if (Style.GetFloatNullable(StyleType.Height) == null)
                height = bounds.Height + _padding * 4;
            else
                height = Style.GetFloat(StyleType.Height) + _padding * 4;
            
            return new Vector2f(width, height);
        }
        
        public override void OnMouseButtonPressed(object sender, MouseButtonEventArgs ev)
        {
            if (ev.Button == Mouse.Button.Right)
            {
                Game.Scene.GameObjects.Remove(this);
                Game.Scene.FocusedObject = null;
            }
        }
        

        public override void OnMouseDrag(object sender, MouseMoveEventArgs ev)
        {
            X = _position.X;
            Y = _position.Y;
            
            _position.X = ev.X;
            _position.Y = ev.Y;
        }
    }
}