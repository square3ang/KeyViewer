using DG.Tweening;

namespace KeyViewer.Models
{
    public struct EaseConfig
    {
        public EaseConfig(Ease ease, float duration, float shrinkFactor)
        {
            Ease = ease;
            Duration = duration;
            ShrinkFactor = shrinkFactor;
        }
        public Ease Ease;
        public float Duration;
        public float ShrinkFactor;
    }
}
