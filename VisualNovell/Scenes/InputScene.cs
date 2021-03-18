using Mirai.Abstracts;
using Mirai.Objects;
using Mirai.Scenes;

namespace VisualNovell.Scenes
{
    public class InputScene : TextScene
    {
        public override string Name { get; set; } = "Input";

        public InputScene()
        {
            Background = new BackgroundObject("textures/backgrounds/0");
        }
        
        public override void Init()
        {
            Input("Введи свое имя:").OnSelect("input2");
        }
    }
}