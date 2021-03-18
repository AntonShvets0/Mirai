using System.Linq;
using Mirai.Abstracts;
using Mirai.ByteSaver;
using Mirai.Exceptions;
using Mirai.Novel.Models;
using Mirai.Scenes;
using SFML.Graphics;

namespace Mirai.Novel.Saver
{
    public class GameSaver
    {
        public static void Load(string file)
        {
            var saver = SaveManager.GetFromFile<SaveData>(file);
            var scene = Scene.Find(saver.Scene);
            scene.Args = saver.Args;

            if (scene is not TextScene)
            {
                throw new MiraiSaveException("Scene is not text scene");
            }

            Game.Scene = scene;

            var textScene = scene as TextScene;
            for (int i = 0; i < saver.Index; i++)
            {
                foreach (var action in textScene.NovelInfo[i].UpdateActions)
                {
                    action();
                }
            }
            
            textScene.Index = saver.Index;

            if (textScene.NovelInfo[textScene.Index] is NovelText text)
            {
                text.Index = saver.TextIndex;
                text.TextObject.TextTyped.DisplayedString 
                    = string.Join(" ", text.Text.Take(text.Index));
                text.TextObject.Text =
                    text.TextObject.TextTyped.DisplayedString;
            }
            
            textScene.Load();
        }

        public static void Save(string file)
        {
            var scene = Game.Scene;
            if (scene is not TextScene)
            {
                throw new MiraiSaveException("Scene is not text scene");
            }

            var data = new SaveData
            {
                Args = scene.Args,
                Index = (scene as TextScene).Index,
                Scene = scene.Name
            };

            if ((scene as TextScene).NovelInfo[data.Index] is NovelText text)
            {
                data.TextIndex = text.Index;
            }
            
            SaveManager.SaveToFile(file, data);
        }
    }
}