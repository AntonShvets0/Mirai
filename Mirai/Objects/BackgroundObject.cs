using System.Collections.Generic;
using Mirai.Abstracts;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai.Objects
{
    public class BackgroundObject : GameObject
    {
        public override Cursor FocusedCursor { get; set; }
        public override bool IsFocusable { get; set; } = false;

        private RectangleShape _background;

        public string Texture;

        public BackgroundObject(string texture)
        {
            Texture = texture;
        }

        public override IEnumerable<Drawable> Update(RenderWindow renderWindow)
        {
            _background ??= new RectangleShape
            {
                Position = new Vector2f(0, 0),
                Size = new Vector2f(1920, 1080),
            };

            _background.Texture = Cache.GetTexture(Texture);
            
            yield return Handle(_background);
        }
    }
}