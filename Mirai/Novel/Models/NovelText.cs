using System;
using System.Collections.Generic;
using Mirai.Abstracts;
using Mirai.Objects;
using Mirai.Scenes;
using SFML.Graphics;
using SFML.System;

namespace Mirai.Novel.Models
{
    public class NovelText : INovel
    {
        public CharacterObject Character { get; set; }

        public string[] Text;
        public int Index;
        
        public List<Action> UpdateActions { get; set; } = new List<Action>();
        
        public NovelText OnUpdate(Action action)
        {
            UpdateActions.Add(action);
            return this;
        }

        public void Update(TextScene scene)
        {
            
        }

        public bool HandleFrame(TextScene scene)
        {
            BoxObject.UpperText = Character.Id;

            if (Index < Text.Length)
            {
                if (Index != 0)
                {
                    TextObject.Text += " " + Text[Index];
                }
                else
                {
                    TextObject.Text = Text[Index];
                    TextObject.TextTyped.DisplayedString = "";
                }

                Index++;
                if (Index < Text.Length) return false;
            }

            return true;
        }

        public BoxObject BoxObject;

        public AnimatedTextObject TextObject;

        public NovelText()
        {
            var text = new Text("", Cache.GetFont("fonts/arial"))
            {
                FillColor = Color.Black,
                CharacterSize = 25
            };
            
            BoxObject = new BoxObject(text);
            TextObject = new AnimatedTextObject(text, new Vector2f());
        }


        public GameObject[] Init(TextScene scene)
        {
            TextObject.Position = new Vector2f(15, 1080 - BoxObject.RectangleShape.Size.Y + 40);
            return new GameObject[]
            {
                BoxObject,
                TextObject
            };
        }
    }
}