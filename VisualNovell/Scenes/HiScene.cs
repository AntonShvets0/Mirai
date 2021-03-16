using Mirai;
using Mirai.Abstracts;
using Mirai.Objects;
using Mirai.Styles;
using SFML.Graphics;
using SFML.System;

namespace VisualNovell.Scenes
{
    public class HiScene : Scene
    {
        public override string Name { get; set; } = "hi";

        public override void Init()
        {
            GameObjects.Add(
                new ButtonObject(new Text("Перейти к сцене", Cache.GetFont("fonts/arial"), 15), new Style())
                {
                    LeftClickHandler = () =>
                    {
                        Game.Scene = Find("Test");
                    },
                    Position = new Vector2f(500, 500)
                });
        }
    }
}