namespace Mirai.Animation
{
    public class Animator
    {
        public string Name;

        public int? Frame = null;
        public int Frames;
        
        public IAnimatorController AnimatorController;
        public AnimatorState State { get; set; }

        public bool IsLooped = true;
        
        public delegate void AnimatorDelegate(Animator animator);
        public AnimatorDelegate EndAnimation;
        public AnimatorDelegate HandleFrame;

        public Animator(
            string name, 
            int frames, 
            AnimatorDelegate handleFrame, 
            AnimatorDelegate endAnimation, 
            IAnimatorController controller)
        {
            Frames = frames;
            HandleFrame = handleFrame;
            EndAnimation = endAnimation;
            AnimatorController = controller;
            Name = name;
        }

        public void OnEndAnimation()
        {
            Frame = 0;
            EndAnimation?.Invoke(this);
        }
    }
}