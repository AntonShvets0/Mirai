using System.Collections.Generic;
using Mirai.Novell.Interfaces;
using Mirai.Novell.Objects;
using SFML.Graphics;
using SFML.System;

namespace Mirai.Novell.States
{
    public class TextState : ISceneState
    {
        public List<TextData> Texts = new List<TextData>();
        
        public int IndexText = 0;
        
        public static Scene.DesignDelegate Design;

        public TextState(TextData data)
        {
            Texts.Add(data);
        }

        public TextState(List<TextData> data)
        {
            Texts = data;
        }
        
        public ISceneState Update(double deltaTime)
        {
            if (Design != null)
            {
                Design(deltaTime, this);
                return this;
            }

            var characters = Texts[IndexText].Characters;
            foreach (var character in characters)
            {
                Game.Instance.Window.Draw(new Sprite
                {
                    Texture = Game.Instance.DataStorage.GetTexture($"{character.Texture}_{character.Emotion}.png"),
                    Position = character.Position
                });
            }
            
            if (Texts[IndexText].Text[Texts[IndexText].IndexText].Length > 0)
            {
                Game.Instance.Window.Draw(new RectangleShape
                {
                    Size = new Vector2f(800f, 170f),
                    FillColor = new Color(100, 100, 100, 125),
                    Position = new Vector2f(0f, 430f)
                }); 
                
                Game.Instance.Window.Draw(new RectangleShape
                {
                    Size = new Vector2f(780f, 60f),
                    FillColor = new Color(100, 100, 100, 160),
                    Position = new Vector2f(10f, 370f)
                });

                var speaker = Texts[IndexText].Speaker;
                Game.Instance.Window.Draw(new Text(speaker.Name, Game.Instance.DataStorage.GetFont("Roboto-Regular.ttf"), 32)
                {
                    FillColor = speaker.Color,
                    Position = new Vector2f(15f, 382f)
                });
                
                Game.Instance.Window.Draw(new Text(BuildText(), Game.Instance.DataStorage.GetFont("Roboto-Regular.ttf"), 20)
                {
                    FillColor = Color.White,
                    Position = new Vector2f(15f, 435f)
                });
            }
            
            return this;
        }

        public string BuildText()
        {
            var text = "";

            for (int i = 0; i <= Texts[IndexText].IndexText; i++)
            {
                text += Texts[IndexText].Text[i];
            }

            return text;
        }
    }
}