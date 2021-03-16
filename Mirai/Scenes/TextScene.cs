using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirai.Abstracts;
using Mirai.Interfaces;
using Mirai.NovelPart;
using Mirai.Objects;
using Mirai.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai.Scenes
{
    public abstract class TextScene : Scene
    {
        public BackgroundObject Background;
        public List<INovelData> NovelInfo = new List<INovelData>();
        private int _index = 0;

        protected BoxObject BoxObject;

        public override void Init()
        {
            GameObjects.Add(Background);
            GameObjects.Add(BoxObject);
            _isFirst = true;
            _index = 0;
            NovelInfo.Clear();
        }

        private bool _isFirst = true;

        public override void Update()
        {
            if (_isFirst) OnMouseButtonPressed(null, new MouseButtonEventArgs(new MouseButtonEvent())
            {
                Button = Mouse.Button.Left
            });

            _isFirst = false;
        }

        public override void OnMouseButtonPressed(object sender, MouseButtonEventArgs ev)
        {
            if (ev.Button != Mouse.Button.Left) return;
            if (_index > NovelInfo.Count - 1)
            {
                Jump(Game.DefaultScene);
                return;
            }

            var part = NovelInfo[_index];
            if (part is NovelText text)
            {
                BoxObject.CharacterObject = text.Character;
                if (text.Index < text.Text.Length)
                {
                    if (text.Index != 0)
                    {
                        BoxObject.AnimatedTextObject.Text += " " + text.Text[text.Index];
                    }
                    else
                    {
                        BoxObject.AnimatedTextObject.Text = text.Text[text.Index];
                        BoxObject.AnimatedTextObject.TextTyped.DisplayedString = "";
                    }

                    text.Index++;
                }
            }

            foreach (var action in part.Actions)
            {
                action();
            }

            if (part is NovelText novelText && novelText.Index < novelText.Text.Length) return;
            _index++;
        }
    }
}