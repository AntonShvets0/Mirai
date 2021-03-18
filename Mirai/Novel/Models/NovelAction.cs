using System;
using System.Collections.Generic;
using Mirai.Abstracts;
using Mirai.Scenes;

namespace Mirai.Novel.Models
{
    public class NovelAction : INovel
    {
        public List<Action> UpdateActions { get; set; } = new List<Action>();

        public NovelAction OnUpdate(Action action)
        {
            UpdateActions.Add(action);
            return this;
        }

        public bool HandleFrame(TextScene scene)
        {
            return true;
        }

        public GameObject[] Init(TextScene scene)
        {
            return Array.Empty<GameObject>();
        }

        public void Update(TextScene scene)
        {
            
        }
    }
}