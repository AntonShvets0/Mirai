using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic;
using Mirai.Abstracts;
using Mirai.Novel;
using Mirai.Novel.Models;
using Mirai.Novel.Saver;
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
        public List<INovel> NovelInfo = new List<INovel>();
        public int Index;
        
        public override void PreInit()
        {
            GameObjects.Add(Background);
            GameObjects.Add(
                new ButtonObject(new Text("Сохранить", Cache.GetFont("fonts/arial"), 21), 
                    new Style(new Dictionary<StyleType, object>
                    {
                        {StyleType.TextColor, Color.Black}
                    }))
                {
                    Position = new Vector2f(10, 920),
                    LeftClickHandler = () =>
                    {
                        GameSaver.Save("test.save");

                        var box = new MessageBoxObject(new Style(), new Text("Вы сохранили игру!", Cache.GetFont("fonts/arial")));
                        GameObjects.Add(box);
                    }
                });

            _isFirst = true;
            Index = 0;
            NovelInfo.Clear();
        }

        public override void PostInit()
        {
            Load();
        }

        private bool _isFirst = true;

        public override void Update()
        {
            if (_isFirst) OnMouseButtonPressed(null, new MouseButtonEventArgs(new MouseButtonEvent())
            {
                Button = Mouse.Button.Left
            });

            _isFirst = false;
            
            if (Index < NovelInfo.Count)
            {
                NovelInfo[Index].Update(this);
            }
        }

        public override void OnMouseButtonPressed(object sender, MouseButtonEventArgs ev)
        {
            if (FocusedObject != null) return;
            if (ev.Button != Mouse.Button.Left) return;
            if (Index > NovelInfo.Count - 1)
            {
                Jump(Game.DefaultScene);
                return;
            }

            var part = NovelInfo[Index];
            foreach (var action in part.UpdateActions)
                action();

            var response = part.HandleFrame(this);
            
            if (Index < NovelInfo.Count && !_isFirst)
                Load();
            
            if (!response) return;
            
            Index++;
        }

        public NovelSelect Select(params string[] strings)
        {
            var select = new NovelSelect
            {
                SelectButtons = strings
            };
            
            NovelInfo.Add(select);
                
            return select;
        }

        public NovelInput Input(string text)
        {
            var input = new NovelInput(text);
            NovelInfo.Add(input);
            
            return input;
        }
        
        
        public void Load()
        {
            if (Index > NovelInfo.Count - 1) return;
            GameObjects.RemoveAll(gameObject => gameObject.Tags.Contains("initObject"));
            var objects = NovelInfo[Index].Init(this);
            foreach (var obj in objects)
            {
                obj.Tags.Add("initObject");
            }
                
            GameObjects.AddRange(objects);
        }
    }
}