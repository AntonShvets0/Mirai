using System.Collections.Generic;

namespace Mirai.Animation
{
    public interface IAnimatorController
    {
        public ICollection<Animator> Animations { get; set; }
        
        public string Animation { get; set; }
    }
}