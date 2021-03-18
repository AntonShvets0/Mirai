using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mirai.Abstracts;
using Mirai.Animation;
using Mirai.ByteSaver.Protocol;
using Mirai.Exceptions;
using Mirai.Models;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirai
{
    public class Game
    {
        private static Scene _currentScene;
        public static Scene Scene
        {
            get => _currentScene;
            set
            {
                _currentScene = value;

                _currentScene.RenderWindow ??= _renderWindow;
                _currentScene.GameObjects.Clear();
                _currentScene.PreInit();
                _currentScene.Init();
                _currentScene.PostInit();
            } 
        }

        public static string DefaultScene { get; private set; }
        
        private static RenderWindow _renderWindow;
        
        public Game(string startScene, VideoMode video, SceneAssembly sceneAssembly)
        {
            Cache.Load();
            Packet.Assembly = sceneAssembly.Assembly;
            
            _renderWindow = new RenderWindow(video, "Mirai Engine");

            LoadScenes(sceneAssembly);
            Scene = Scene.Find(startScene);
            DefaultScene = startScene;
            
            _renderWindow.Closed += OnClosed;
            _renderWindow.Resized += OnResized;
            _renderWindow.KeyPressed += OnKeyPressed;
            _renderWindow.KeyReleased += OnKeyReleased;
            _renderWindow.MouseEntered += OnMouseEntered;
            _renderWindow.MouseLeft += OnMouseLeft;
            _renderWindow.MouseMoved += OnMouseMoved;
            _renderWindow.MouseButtonPressed += OnMouseButtonPressed;
            _renderWindow.MouseButtonReleased += OnMouseButtonReleased;
            _renderWindow.MouseWheelScrolled += OnMouseWheelScrolled;
            _renderWindow.TextEntered += OnTextEntered;
        }

        public void Init()
        {
            var delta = new Stopwatch();
            var framesCounter = new Clock();

            while (_renderWindow.IsOpen)
            {
                _renderWindow.DispatchEvents();
                var elapsed = delta.ElapsedMilliseconds;
                delta.Restart();

                _currentScene.RenderWindow = _renderWindow;
                _currentScene.DeltaTime = elapsed / 1000f;

                _renderWindow.Clear();
                _renderWindow.SetTitle(_currentScene.Title ?? _currentScene.Name);
                
                foreach (var drawableObject in _currentScene.GameObjects.Where(g => g.IsVisible))
                {
                    foreach (var drawable in drawableObject.Update(_renderWindow))
                    {
                        _renderWindow.Draw(drawable);
                    }

                    HandleAnimation(drawableObject);
                }
                
                _currentScene.Update();
                
                delta.Stop();
                
                var currentTime = framesCounter.Restart().AsSeconds();
                var fps = 1f / currentTime;
                _renderWindow.Draw(new Text(fps.ToString(), Cache.GetFont("fonts/arial"), 12)
                {
                    FillColor = Color.Red
                });
                
                _renderWindow.Display();
            }
        }

        private void HandleAnimation(GameObject gameObject)
        {
            if (gameObject == null) return;
            
            if (gameObject is IAnimatorController animator && animator.Animation != null)
            {
                var animation = animator.Animations.FirstOrDefault(a => a.Name == animator.Animation);

                if (animation == null)
                {
                    throw new MiraiAnimationException("Unknown animation!");
                }

                if (animation.Frame == null)
                {
                    if (animation.State == AnimatorState.Normal) animation.Frame = 0;
                    else animation.Frame = animation.Frames - 1;
                }
                        
                if (animation.State == AnimatorState.Normal)
                    animation.Frame++;
                else
                    animation.Frame--;

                animation.HandleFrame(animation);

                if (animation.Frames - 1 < animation.Frame || animation.Frame < 1)
                {
                    if (animation.IsLooped && animation.State == AnimatorState.Normal) animation.Frame = 0;
                    else if (animation.IsLooped) animation.Frame = animation.Frames - 1;
                    else
                    {
                        animation.OnEndAnimation();
                        animation.Frame = null;

                        animator.Animation = null;
                    }
                }
            }
        }
        
        private void LoadScenes(SceneAssembly sceneAssembly)
        {
            foreach (var obj in 
                sceneAssembly.Assembly
                    .GetTypes()
                    .Where(c => c.IsClass && !c.IsAbstract && c.IsSubclassOf(typeof(Scene)) 
                                && sceneAssembly.NamespaceScenes.Contains(c.Namespace))
            )
            {
                if (obj.IsAbstract) continue;
                Activator.CreateInstance(obj);
            }
        }

        public void OnClosed(object sender, EventArgs ev)
        {
            if (!(_currentScene?.OnClosed(sender, ev) ?? true))
            {
                return;
            }
            
            _renderWindow.Close();
            Environment.Exit(0);
        }

        private void OnResized(object sender, SizeEventArgs ev)
        {
            _currentScene?.OnResized(sender, ev);
        }

        private void OnKeyPressed(object sender, KeyEventArgs ev)
        {
            _currentScene?.FocusedObject?.OnKeyPressed(sender, ev);
            _currentScene?.OnKeyPressed(sender, ev);
        }

        private void OnKeyReleased(object sender, KeyEventArgs ev)
        {
            _currentScene.FocusedObject?.OnKeyReleased(sender, ev);
            _currentScene.OnKeyReleased(sender, ev);
        }

        private void OnMouseEntered(object sender, EventArgs ev)
        {
            _currentScene?.OnMouseEntered(sender, ev);
        }
        
        private void OnMouseLeft(object sender, EventArgs ev)
        {
            _currentScene?.OnMouseLeft(sender, ev);
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs ev)
        {
            var coords = _renderWindow.MapPixelToCoords(new Vector2i(ev.X, ev.Y));
            var obj = _currentScene.GameObjects.FirstOrDefault(o => 
                o.IsFocusable 
                && o.X <= coords.X + o.SizeX && o.X >= coords.X - o.SizeX
                && o.Y <= coords.Y + o.SizeY && o.Y >= coords.Y - o.SizeY
            );

            if (obj != null && _currentScene.FocusedObject == obj)
            {
                if (!_moving) obj.OnMouseMoved(sender, ev);
                else obj.OnMouseDrag(sender, ev);
                
                _renderWindow.SetMouseCursor(_currentScene.FocusedObject.FocusedCursor);
            }
            else if (obj != null && !(_currentScene.FocusedObject?.IsLocked ?? false))
            {
                _currentScene.FocusedObject?.OnMouseLeft(sender, ev);
                
                _currentScene.FocusedObject = obj;
                _currentScene.FocusedObject.OnMouseEntered(sender, ev);
                _renderWindow.SetMouseCursor(_currentScene.FocusedObject.FocusedCursor);
            }
            else if (!(_currentScene.FocusedObject?.IsLocked ?? false))
            {
                _currentScene?.FocusedObject?.OnMouseLeft(sender, ev);

                _currentScene.FocusedObject = null;
                _renderWindow.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow));
            }

            if (!_moving) _currentScene?.OnMouseMoved(sender, ev);
            else _currentScene?.OnMouseDrag(sender, ev);
        }

        private bool _moving = false;
        
        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs ev)
        {
            var old = _currentScene;
            
            _currentScene.FocusedObject?.OnMouseButtonPressed(sender, ev);

            if (_currentScene != old) return;
            
            _currentScene?.OnMouseButtonPressed(sender, ev);

            if (ev.Button == Mouse.Button.Left)
            {
                _moving = true;
            }
        }

        private void OnMouseButtonReleased(object sender, MouseButtonEventArgs ev)
        {
            _currentScene.FocusedObject?.OnMouseButtonReleased(sender, ev);
            _currentScene?.OnMouseButtonReleased(sender, ev);

            if (ev.Button == Mouse.Button.Left)
            {
                _moving = false;
            }
        }

        private void OnMouseWheelScrolled(object sender, MouseWheelScrollEventArgs ev)
        {
            _currentScene.FocusedObject?.OnMouseWheelScrolled(sender, ev);
            _currentScene?.OnMouseWheelScrolled(sender, ev);
        }

        private void OnTextEntered(object sender, TextEventArgs ev)
        {
            _currentScene.FocusedObject?.OnTextEntered(sender, ev);
            _currentScene?.OnTextEntered(sender, ev);
        }
    }
}