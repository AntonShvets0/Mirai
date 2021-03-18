using System;
using System.Collections.Generic;
using Mirai.Abstracts;
using Mirai.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai.Objects
{
    public class InputObject : GameObject
    {
        public Text Text;
        public Vector2f Position;

        public override bool IsLocked { get; set; } = true;
        public override Cursor FocusedCursor { get; set; } = new Cursor(Cursor.CursorType.Arrow);
        public override bool IsFocusable { get; set; } = false;

        public Action EnterHandler;
        private int _pointer;
        private Text _pointerReference;

        public int Pointer
        {
            get => _pointer;
            set
            {
                if (value > Text.DisplayedString.Length - 1) return;
                if (value < 0) return;
                
                _pointer = value;
                
                SetPointer();
            }
        }

        public Style Style;

        public InputObject(Text text, Vector2f position, Style style)
        {
            Text = text;
            Position = position;
            Text.Position = Position;
            _pointerReference = new Text(text);
            _pointerReference.DisplayedString = "|";
            
            Style = style;
            _pointerReference.FillColor = style.GetColor(StyleType.PointerColor) ?? Color.Blue;
            
            Pointer = Text.DisplayedString.Length - 1;
            SetPointer();
        }

        private bool _isShowPointer = true;

        public override IEnumerable<Drawable> Update(RenderWindow renderWindow)
        {
            if (_isShowPointer)
            {
                _pointerReference.FillColor = new Color(_pointerReference.FillColor.R, _pointerReference.FillColor.G, _pointerReference.FillColor.B, (byte)(_pointerReference.FillColor.A - 3));
                if (_pointerReference.FillColor.A < 1) _isShowPointer = false;
            }
            else
            {
                _pointerReference.FillColor = new Color(_pointerReference.FillColor.R, _pointerReference.FillColor.G, _pointerReference.FillColor.B, (byte)(_pointerReference.FillColor.A + 3));
                if (_pointerReference.FillColor.A >= 255) _isShowPointer = true;
            }

            Text.Position = Position;
            yield return Text;
            if (Game.Scene.FocusedObject == this) yield return _pointerReference;
        }

        public override void OnTextEntered(object sender, TextEventArgs ev)
        {
            if (char.IsControl(ev.Unicode[0])) return;
            
            if (Pointer < 0 || Text.DisplayedString.Length == 0)
            {
                Text.DisplayedString += ev.Unicode;
            }
            else
            {
                Text.DisplayedString = Text.DisplayedString.Substring(0, _pointer + 1) + ev.Unicode +
                                       Text.DisplayedString.Substring(_pointer + 1);
            }
                
            Pointer++;
        }

        public override void OnKeyPressed(object sender, KeyEventArgs ev)
        {
            if (ev.Code == Keyboard.Key.Backspace)
            {
                if (Pointer < 0 || Text.DisplayedString.Length == 0)
                {
                        
                }
                else
                {
                    var newString =
                        _pointer + 1 <= Text.DisplayedString.Length - 1
                            ? Text.DisplayedString.Substring(_pointer + 1)
                            : "";
                        
                    var str = Text.DisplayedString.Length >= _pointer
                        ? Text.DisplayedString.Substring(0, _pointer)
                        : "";

                    Text.DisplayedString = str + newString;
                    Pointer--;
                }
            }
            else if (ev.Code == Keyboard.Key.Enter)
            {
                EnterHandler?.Invoke();
            }
            else if (ev.Code == Keyboard.Key.Left)
            {
                Pointer--;
            }
            else if (ev.Code == Keyboard.Key.Right)
            {
                Pointer++;
            }
            else if (ev.Code == Keyboard.Key.Enter)
            {
                EnterHandler?.Invoke();
            }
        }

        private void SetPointer()
        {
            var position = Text.Position;
            var pos = Text.FindCharacterPos((uint) _pointer).X;
            var x = position.X + pos + ((Text.FindCharacterPos((uint)_pointer + 1).X - pos) / 1.3f);

            _pointerReference.Position = new Vector2f(x, position.Y);
        }
    }
}