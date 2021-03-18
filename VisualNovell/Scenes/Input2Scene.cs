using Mirai.Enums;
using Mirai.Objects;
using Mirai.Scenes;

namespace VisualNovell.Scenes
{
    public class Input2Scene : TextScene
    {
        public override string Name { get; set; } = "input2";

        public Input2Scene()
        {
            Background = new BackgroundObject("textures/backgrounds/0");
        }
        
        public override void Init()
        {
            var miura = new CharacterObject("miura1", "textures/characters/example");
            miura.Show(Direction.Center, Emotion.Normal, CharacterTypeShowing.Fade);
            miura.Say("Хорошо!", $"Привет, {Args}!");
        }
    }
}