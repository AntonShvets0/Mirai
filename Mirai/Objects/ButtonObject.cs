using System;
using System.Collections.Generic;
using System.Linq;
using Mirai.Abstracts;
using Mirai.Animation;
using Mirai.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai.Objects
{
    public class ButtonObject : GameObject, IAnimatorController
    {
        public override bool IsFocusable { get; set; } = true;
        public override Cursor FocusedCursor { get; set; } = new Cursor(Cursor.CursorType.Hand);

        private string _animation;
        public string Animation
        {
            get
            {
                return _animation;
            }
            set
            {
                _animation = value;
            }
        }

        public ICollection<Animator> Animations { get; set; }

        public Text Text;
        public Style Style;
        public Vector2f Position = new Vector2f(0f, 0f);
        
        public Action LeftClickHandler;
        public Action RightClickHandler;
        public Action HoverHandler;

        private float _padding;
        private Color _defaultColor;
        private Color? _modifiedColor;
        
        private RectangleShape _shape;

        public ButtonObject(Text text, Style style)
        {
            Style = style;
            Text = text;
            
            Animations = new List<Animator>
            {
                new Animator("hover", 75, animator =>
                {
                    _modifiedColor = new Color(
                        (byte)(_defaultColor.R - animator.Frame), 
                        (byte)(_defaultColor.G - animator.Frame), 
                        (byte)(_defaultColor.B - animator.Frame));
                }, animator =>
                {
                }, this) { IsLooped = false }
            };
        }

        public override IEnumerable<Drawable> Update(RenderWindow renderWindow)
        {
            _padding = Style.GetFloatNullable(StyleType.Padding) ?? Text.CharacterSize / 5f;
            _defaultColor = Style.GetColor(StyleType.Color) ?? Color.White;
            _shape = new RectangleShape(CalculateSize(renderWindow))
            {
                FillColor = _modifiedColor ?? _defaultColor,
                OutlineColor = Style.GetColor(StyleType.BorderColor) ?? Color.Black,
                OutlineThickness = Style.GetIntNullable(StyleType.BorderSize) ?? 1,
                Position = Position
            };
            
            yield return Handle(_shape);

            Text.Position = CalculateTextPosition();
            Text.FillColor = Style.GetColor(StyleType.TextColor) ?? Color.Black;

            yield return Handle(Text);
        }

        private Vector2f CalculateTextPosition()
        {
            return new Vector2f(Position.X + _padding, Position.Y + _padding / 3);
        }

        private Vector2f CalculateSize(RenderWindow renderWindow)
        {
            float width;
            float height;

            var bounds = Text.GetLocalBounds();
            if (Style.GetFloatNullable(StyleType.Width) == null)
                width = bounds.Width + _padding * 2;
            else
                width = Style.GetFloat(StyleType.Width) + _padding * 2;

            if (Style.GetFloatNullable(StyleType.Height) == null)
                height = bounds.Height + _padding * 2;
            else
                height = Style.GetFloat(StyleType.Height) + _padding * 2;
            
            return new Vector2f(width, height);
        }

        public override void OnMouseButtonPressed(object sender, MouseButtonEventArgs ev)
        {
            if (ev.Button == Mouse.Button.Left) LeftClickHandler?.Invoke();
            else RightClickHandler?.Invoke();
        }

        public override void OnMouseEntered(object sender, EventArgs ev)
        {
            Animations.First(a => a.Name == "hover").State = AnimatorState.Normal;
            Animation = "hover";
            HoverHandler?.Invoke();
        }

        public override void OnMouseLeft(object sender, EventArgs ev)
        {
            Animations.First(a => a.Name == "hover").State = AnimatorState.Reversed;
            Animation = "hover";
        }
    }
}