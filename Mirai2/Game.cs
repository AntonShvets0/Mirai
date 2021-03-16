using System;
using System.Diagnostics;
using System.Threading;
using Mirai.Novell;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai
{
    public class Game
    {
        public RenderWindow Window;
        public Scene CurrentScene;
        public DataStorage DataStorage = new DataStorage();
        public Config Config;

        public static Game Instance;
        
        public Game(string title, Config config)
        {
            Instance = this;
            Config = config;

            new T();
            
            Window = new RenderWindow(new VideoMode(800, 600), title);
            Window.SetVerticalSyncEnabled(true);
            Window.Closed += OnClose;
            Window.Mouse += OnClose;

            DataStorage.LoadSounds();
            DataStorage.LoadFonts();
            DataStorage.LoadTextures();
        }

        public void Init()
        {
            var delta = new Stopwatch();

            while (Window.IsOpen)
            {
                Window.DispatchEvents();
                
                var elapsed = delta.ElapsedMilliseconds;
                
                delta.Restart();
                Update(elapsed / 1000);
                delta.Stop();
                
                Window.Display();
            }
        }
        
        private void Update(double deltaTime)
        {
            Window.Clear();
            CurrentScene?.Update(deltaTime);
        }

        private void OnClose(object sender, EventArgs ev)
        {
            Window.Close();
            Environment.Exit(0);
        }
    }
}