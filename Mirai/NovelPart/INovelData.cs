using System;
using System.Collections.Generic;
using Mirai.Objects;

namespace Mirai.NovelPart
{
    public interface INovelData
    {
        public CharacterObject Character { get; set; }

        public List<Action> Actions { get; set; }
    }
}