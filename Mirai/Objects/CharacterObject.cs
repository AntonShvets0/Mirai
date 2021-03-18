using System;
using System.Collections.Generic;
using System.Linq;
using Mirai.Abstracts;
using Mirai.Enums;
using Mirai.Novel.Models;
using Mirai.Scenes;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai.Objects
{
    public class CharacterObject : GameObject
    {
        public override bool IsFocusable { get; set; } = false;
        public override Cursor FocusedCursor { get; set; } = new Cursor(Cursor.CursorType.Arrow);

        public string Id;
        public string Texture;
        public Color Color;
        public Direction Direction;
        public Emotion Emotion;

        private Sprite _sprite;
        private bool _isHide = false;

        private static HashSet<CharacterObject> _characters = new HashSet<CharacterObject>();
        public static CharacterObject Find(string id) => _characters.FirstOrDefault(c => c.Id == id);
        
        public CharacterObject(string id, string texture, Color color)
        {
            Id = id;
            Texture = texture;
            Color = color;
            IsVisible = false;
            _characters.Add(this);
        }
        
        public CharacterObject(string id, string texture)
        {
            Id = id;
            Texture = texture;
            Color = Color.Black;
            IsVisible = false;
            _characters.Add(this);
        }

        public void Show(
            Direction direction = Direction.Left, 
            Emotion emotion = Emotion.Normal, 
            CharacterTypeShowing type = CharacterTypeShowing.Normal,
            float positionOffset = 0
            )
        {
            var text = (Game.Scene as TextScene).NovelInfo.LastOrDefault();
            if (text == null)
            {
                text = new NovelAction();
                (Game.Scene as TextScene).NovelInfo.Add(text);
            }
            
            text.UpdateActions.Add(() =>
            {
                if (_sprite == null)
                {
                    _sprite = new Sprite(Cache.GetTexture($"{Texture}_{(int) emotion}"));

                    switch (direction)
                    {
                        case Direction.Left:
                            _sprite.Position = new Vector2f
                            (
                                (float)(1920 * 0.1 + positionOffset), 
                                1080 - _sprite.TextureRect.Height
                            );
                            break;
                        case Direction.Right:
                            _sprite.Position = new Vector2f
                            (
                                (float)(1920 - 1920 * 0.1 + positionOffset), 
                                1080 - _sprite.TextureRect.Height 
                            );
                            break;
                        case Direction.Center:
                            _sprite.Position = new Vector2f
                            (
                                (1920 / 2f - _sprite.TextureRect.Width / 2f + positionOffset), 
                                1080 - _sprite.TextureRect.Height 
                            );
                        
                        
                            break;
                    }
                }
                if (type == CharacterTypeShowing.Fade)
                {
                    _sprite.Color = new Color(_sprite.Color.R, _sprite.Color.G, _sprite.Color.B, 0);
                }

                if (!Game.Scene.GameObjects.Contains(this))
                {
                    Game.Scene.GameObjects.Insert(1, this);
                }
            
                _isHide = false;
                IsVisible = true; 
            });
        }
        

        public void Hide(CharacterTypeShowing typeShowing = CharacterTypeShowing.Normal)
        {
            var text = (Game.Scene as TextScene).NovelInfo.LastOrDefault();
            if (text == null)
            {
                text = new NovelAction();
                (Game.Scene as TextScene).NovelInfo.Add(text);
            }

            text.UpdateActions.Add(() =>
            {
                if (typeShowing == CharacterTypeShowing.Fade) _isHide = true;
                else IsVisible = false;
            });
        }

        public NovelText Say(params string[] str)
        {
            var textScene = Game.Scene as TextScene;

            var action = new NovelText
            {
                Text = str,
                Character = this
            };
            
            textScene.NovelInfo.Add(action);

            return action;
        }

        public override IEnumerable<Drawable> Update(RenderWindow renderWindow)
        {
            if (_sprite == null) yield break;
            
            if (_sprite.Color.A != 255 && !_isHide)
            {
                _sprite.Color = new Color(
                    _sprite.Color.R, 
                    _sprite.Color.G, 
                    _sprite.Color.B, 
                    (byte)(_sprite.Color.A + 1));
            }

            if (_sprite.Color.A != 0 && _isHide)
            {
                _sprite.Color = new Color(
                    _sprite.Color.R, 
                    _sprite.Color.G, 
                    _sprite.Color.B, 
                    (byte)(_sprite.Color.A - 1));
            } 
            else if (_isHide)
            {
                IsVisible = false;
            }

            _sprite.Texture = Cache.GetTexture($"{Texture}_{(int)Emotion}");
            yield return Handle(_sprite);
        }
    }
}