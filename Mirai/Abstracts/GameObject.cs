using System;
using System.Collections.Generic;
using System.Linq;
using Mirai.Exceptions;
using SFML.Graphics;
using SFML.Window;

namespace Mirai.Abstracts
{
    public abstract class GameObject
    {
        public abstract bool IsFocusable { get; set; }
        public abstract Cursor FocusedCursor { get; set; }
        public bool IsVisible = true;
        
        public float? X;
        public float? Y;
        
        public float? SizeX;
        public float? SizeY;

        public abstract IEnumerable<Drawable> Update(RenderWindow renderWindow);

        protected Drawable Handle(Transformable transformable)
        {
            if (transformable is Drawable drawable)
            {
                if (X == null || X > transformable.Position.X)
                {
                    X = transformable.Position.X;
                }
                
                if (Y == null || Y > transformable.Position.Y)
                {
                    Y = transformable.Position.Y;
                }

                var sizeX = transformable.Scale.X;
                var sizeY = transformable.Scale.Y;
                
                if (transformable is RectangleShape rectangleShape)
                {
                    sizeX = rectangleShape.Size.X;
                    sizeY = rectangleShape.Size.Y;
                }
                else if (transformable is Sprite sprite)
                {
                    sizeX = sprite.TextureRect.Width;
                    sizeY = sprite.TextureRect.Height;
                }

                if (SizeX == null || SizeX < sizeX)
                {
                    SizeX = sizeX;
                }
                
                if (SizeY == null || SizeY < sizeY)
                {
                    SizeY = sizeY;
                }

                return drawable;
            }

            throw new MiraiDrawableException("This transformable object is not implementing Drawable interface");
        }

        public void Destroy()
        {
            if (Game.Scene.FocusedObject == this) Game.Scene.FocusedObject = null;
            Game.Scene.GameObjects.Remove(this);
        }
        
        public virtual void OnKeyPressed(object sender, KeyEventArgs ev) {}
        public virtual void OnKeyReleased(object sender, KeyEventArgs ev) {}
        public virtual void OnMouseEntered(object sender, EventArgs ev) {}
        public virtual void OnMouseLeft(object sender, EventArgs ev) {}
        public virtual void OnMouseMoved(object sender, MouseMoveEventArgs ev) {}
        public virtual void OnMouseButtonPressed(object sender, MouseButtonEventArgs ev) {}
        public virtual void OnMouseButtonReleased(object sender, MouseButtonEventArgs ev) {}
        public virtual void OnMouseWheelScrolled(object sender, MouseWheelScrollEventArgs ev) {}
        public virtual void OnTextEntered(object sender, TextEventArgs ev) {}

        public virtual void OnMouseDrag(object sender, MouseMoveEventArgs ev) { }
    }
}