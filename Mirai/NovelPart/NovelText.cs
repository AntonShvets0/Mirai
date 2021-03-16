using System;
using System.Collections.Generic;
using Mirai.Objects;

namespace Mirai.NovelPart
{
    public class NovelText : INovelData
    {
        public CharacterObject Character { get; set; }

        public string[] Text;
        public int Index;
        
        
        public List<Action> Actions { get; set; } = new List<Action>();
    }
}