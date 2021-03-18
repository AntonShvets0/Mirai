using System;
using System.Collections.Generic;
using Mirai.Abstracts;
using Mirai.Objects;
using Mirai.Scenes;
using Mirai.Styles;
using SFML.Graphics;
using SFML.System;

namespace Mirai.Novel.Models
{
    public class NovelSelect : INovel
    {
        public List<Action> UpdateActions { get; set; } = new List<Action>();
        public string[] SelectButtons = Array.Empty<string>();

        public Dictionary<string, string> SelectScene = new Dictionary<string, string>();

        public GameObject[] Init(TextScene scene)
        {
            var gameObjects = new List<GameObject>();

            var i = 0;
            
            foreach (var button in SelectButtons)
            {
                var btn = new ButtonObject(new Text(button, Cache.GetFont("fonts/arial")), new Style());
                btn.Position = new Vector2f(1920 / 2f - (btn.RectangleShape.Size.X / 2f), 1080 / 2f + (i * 40));
                btn.LeftClickHandler = () =>
                {
                    if (SelectScene.ContainsKey(button)) Game.Scene = Scene.Find(SelectScene[button]);
                    else Game.Scene = Scene.Find(Game.DefaultScene);
                };

                gameObjects.Add(btn);
                i++;
            }

            return gameObjects.ToArray();
        }

        public void Update(TextScene scene)
        {
            
        }

        public NovelSelect OnUpdate(Action action)
        {
            UpdateActions.Add(action);
            return this;
        }

        public void OnSelect(string button, string scene)
        {
            SelectScene.Add(button, scene);
        }
        
        public bool HandleFrame(TextScene scene)
        {
            return false;
        }
    }
}