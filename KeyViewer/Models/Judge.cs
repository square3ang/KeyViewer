namespace KeyViewer.Models
{
    public class Judge<T>
    {
        public T TooEarly;
        public T VeryEarly;     
        public T EarlyPerfect;
        public T Perfect;
        public T LatePerfect;
        public T VeryLate;
        public T TooLate;
        public T Multipress;
        public T FailMiss;
        public T FailOverload;
        public Judge<T> Copy()
        {
            var newJudge = new Judge<T>();
            newJudge.TooEarly = TooEarly;
            newJudge.VeryEarly = VeryEarly;
            newJudge.EarlyPerfect = EarlyPerfect;
            newJudge.Perfect = Perfect;
            newJudge.LatePerfect = LatePerfect;
            newJudge.VeryLate = VeryLate;
            newJudge.TooLate = TooLate;
            newJudge.Multipress = Multipress;
            newJudge.FailMiss = FailMiss;
            newJudge.FailOverload = FailOverload;
            return newJudge;
        }
    }
}