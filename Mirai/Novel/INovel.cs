using System;
using System.Collections.Generic;
using Mirai.Abstracts;
using Mirai.Scenes;
using SFML.Graphics;

namespace Mirai.Novel
{
    public interface INovel
    {
        public List<Action> UpdateActions { get; set; }

        public bool HandleFrame(TextScene scene);
        public GameObject[] Init(TextScene scene);
        public void Update(TextScene scene);
    }
}