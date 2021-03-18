using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;

namespace Mirai.Abstracts
{
    public abstract class Scene
    {
        public abstract string Name { get; set; }
        public string Title;
        public object Args;

        public double DeltaTime;

        public RenderWindow RenderWindow;
        public List<GameObject> GameObjects = new List<GameObject>();
        public GameObject FocusedObject;

        protected Scene()
        {
            if (!_scenes.Contains(this)) 
                _scenes.Add(this);
        }

        public virtual void PreInit()
        {
        }

        public virtual void Init()
        {
        }

        public virtual void PostInit()
        {
        }

        public virtual void Update()
        {
        }

        public void Jump(string name, object args = null)
        {
            var scene = Find(name);
            if (scene != null) scene.Args = args;

            Game.Scene = scene;
        }

        public virtual bool OnClosed(object sender, EventArgs ev) => true;
        public virtual void OnResized(object sender, SizeEventArgs ev) {}
        public virtual void OnKeyPressed(object sender, KeyEventArgs ev) {}
        public virtual void OnKeyReleased(object sender, KeyEventArgs ev) {}
        public virtual void OnMouseEntered(object sender, EventArgs ev) {}
        public virtual void OnMouseLeft(object sender, EventArgs ev) {}
        public virtual void OnMouseMoved(object sender, MouseMoveEventArgs ev) {}
        public virtual void OnMouseButtonPressed(object sender, MouseButtonEventArgs ev) {}
        public virtual void OnMouseButtonReleased(object sender, MouseButtonEventArgs ev) {}
        public virtual void OnMouseWheelScrolled(object sender, MouseWheelScrollEventArgs ev) {}
        public virtual void OnTextEntered(object sender, TextEventArgs ev) {}
        public virtual void OnMouseDrag(object sender, MouseMoveEventArgs ev) { }

        private static HashSet<Scene> _scenes = new HashSet<Scene>();
        public static Scene Find(string name) => _scenes.FirstOrDefault(s => s.Name == name);
        public static void Remove(string name) => _scenes.Remove(Find(name));
    }
}