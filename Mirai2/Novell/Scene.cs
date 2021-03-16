using System.Collections.Generic;
using System.Linq;
using Mirai.Novell.Interfaces;

namespace Mirai.Novell
{
    public class Scene
    {
        public string Name;
        public ISceneState State;

        public delegate void DesignDelegate(double deltaTime, ISceneState state);
        
        private static List<Scene> _scenes = new List<Scene>();

        public static Scene Find(string name)
        {
            return _scenes.FirstOrDefault(s => s.Name == name);
        }

        public static Scene Add(Scene scene)
        {
            _scenes.Add(scene);
            return scene;
        }

        public Scene(string name, ISceneState state)
        {
            Name = name;
            State = state;
        }

        public void Load()
        {
            Game.Instance.CurrentScene = this;
        }

        public void Update(double deltaTime)
        {
            State = State.Update(deltaTime);
        }
    }
}