using Mirai;
using Mirai.Enums;
using Mirai.Objects;
using Mirai.Scenes;
using SFML.Graphics;

namespace VisualNovell.Scenes
{
    public class TestScene : TextScene
    {
        public override string Name { get; set; } = "Test";

        public override void Init()
        {
            Background = new BackgroundObject("textures/backgrounds/0");
            BoxObject = new BoxObject(new Text("", Cache.GetFont("fonts/arial"))
            {
                FillColor = Color.Black,
                CharacterSize = 25
            });
        
            base.Init();

            var miura = new CharacterObject("miura1", "textures/characters/example");

            miura.Say("Привет!");
            miura.Show(Direction.Left, Emotion.Normal, CharacterTypeShowing.Fade);
            
            miura.Say("Что-ж...", "Впрочем, ты не можешь мне ответить");
        }
    }
}