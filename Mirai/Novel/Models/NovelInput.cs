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
    public class NovelInput : INovel
    {
        public string Text;
        public int Index;
        
        public List<Action> UpdateActions { get; set; } = new List<Action>();
        
        public NovelInput OnUpdate(Action action)
        {
            UpdateActions.Add(action);
            return this;
        }

        public bool HandleFrame(TextScene scene)
        {
            return false;
        }

        public BoxObject BoxObject;

        public InputObject InputObject;

        public NovelInput(string upperText)
        {
            var text = new Text("", Cache.GetFont("fonts/arial"))
            {
                FillColor = Color.Black,
                CharacterSize = 25
            };
            
            BoxObject = new BoxObject(text);
            InputObject = new InputObject(text, new Vector2f(), new Style());
            InputObject.EnterHandler = () =>
            {
                var scene = Abstracts.Scene.Find(Scene);

                if (scene != null)
                {
                    scene.Args = InputObject.Text.DisplayedString;
                    Game.Scene = scene;
                }
                else Game.Scene = Abstracts.Scene.Find(Game.DefaultScene);
            };
            
            
            Text = upperText;
            
            BoxObject.UpperText = Text;
        }

        public string Scene;
        
        public void OnSelect(string scene)
        {
            Scene = scene;
        }

        public GameObject[] Init(TextScene scene)
        {
            InputObject.Position = new Vector2f(15, 1080 - BoxObject.RectangleShape.Size.Y + 40);
            return new GameObject[]
            {
                BoxObject,
                InputObject
            };
        }

        public void Update(TextScene scene)
        {
            if (scene.FocusedObject != InputObject && scene.FocusedObject != null) return;
            
            scene.FocusedObject = InputObject;
        }
    }
}