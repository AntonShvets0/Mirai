using System;
using System.Collections.Generic;
using Mirai;
using Mirai.Enums;
using Mirai.Novel.Saver;
using Mirai.Objects;
using Mirai.Scenes;
using Mirai.Styles;
using SFML.Graphics;
using SFML.System;

namespace VisualNovell.Scenes
{
    public class TestScene : TextScene
    {
        public override string Name { get; set; } = "Test";

        public TestScene()
        {
            Background = new BackgroundObject("textures/backgrounds/0");
        }
        
        public override void Init()
        {
            var miura = new CharacterObject("miura1", "textures/characters/example");

            miura.Say("Привет!");
            miura.Show(Direction.Left, Emotion.Normal, CharacterTypeShowing.Fade);

            miura.Say("Что-ж...", "Кто ты?");
            Select("Ответить", "Уйти").OnSelect("Ответить", "Input");
        }
    }
}