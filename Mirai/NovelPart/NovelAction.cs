using System;
using System.Collections.Generic;
using Mirai.Objects;

namespace Mirai.NovelPart
{
    public class NovelAction : INovelData
    {
        public CharacterObject Character { get; set; } = null;
        
        public List<Action> Actions { get; set; } = new List<Action>();
    }
}