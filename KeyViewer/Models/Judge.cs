using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class Judge<T> : IModel, ICopyable<Judge<T>>
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
        public T Get(HitMargin hitMargin)
        {
            switch (hitMargin)
            {
                case HitMargin.TooEarly: return TooEarly;
                case HitMargin.VeryEarly: return VeryEarly;
                case HitMargin.EarlyPerfect: return EarlyPerfect;
                case HitMargin.Perfect: return Perfect;
                case HitMargin.LatePerfect: return LatePerfect;
                case HitMargin.VeryLate: return VeryLate;
                case HitMargin.TooLate: return TooLate;
                case HitMargin.Multipress: return Multipress;
                case HitMargin.FailMiss: return FailMiss;
                case HitMargin.FailOverload: return FailOverload;
                default: return Perfect;
            }
        }
        public void Set(HitMargin hitMargin, T value)
        {
            switch (hitMargin)
            {
                case HitMargin.TooEarly:
                    TooEarly = value;
                    break;
                case HitMargin.VeryEarly:
                    VeryEarly = value;
                    break;
                case HitMargin.EarlyPerfect:
                    EarlyPerfect = value;
                    break;
                case HitMargin.Perfect:
                    Perfect = value;
                    break;
                case HitMargin.LatePerfect:
                    LatePerfect = value;
                    break;
                case HitMargin.VeryLate:
                    VeryLate = value;
                    break;
                case HitMargin.TooLate:
                    TooLate = value;
                    break;
                case HitMargin.Multipress:
                    Multipress = value;
                    break;
                case HitMargin.FailMiss:
                    FailMiss = value;
                    break;
                case HitMargin.FailOverload:
                    FailOverload = value;
                    break;
                default:
                    Perfect = value;
                    break;
            }
        }
        public JsonNode Serialize()
        {
            JsonNode node = JsonNode.Empty;
            node[nameof(TooEarly)] = ModelUtils.ToNode<T>(TooEarly);
            node[nameof(VeryEarly)] = ModelUtils.ToNode<T>(VeryEarly);
            node[nameof(EarlyPerfect)] = ModelUtils.ToNode<T>(EarlyPerfect);
            node[nameof(Perfect)] = ModelUtils.ToNode<T>(Perfect);
            node[nameof(LatePerfect)] = ModelUtils.ToNode<T>(LatePerfect);
            node[nameof(VeryLate)] = ModelUtils.ToNode<T>(VeryLate);
            node[nameof(TooLate)] = ModelUtils.ToNode<T>(TooLate);
            node[nameof(Multipress)] = ModelUtils.ToNode<T>(Multipress);
            node[nameof(FailMiss)] = ModelUtils.ToNode<T>(FailMiss);
            node[nameof(FailOverload)] = ModelUtils.ToNode<T>(FailOverload);
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            TooEarly = (T)ModelUtils.ToObject<T>(node[nameof(TooEarly)]);
            VeryEarly = (T)ModelUtils.ToObject<T>(node[nameof(VeryEarly)]);
            EarlyPerfect = (T)ModelUtils.ToObject<T>(node[nameof(EarlyPerfect)]);
            Perfect = (T)ModelUtils.ToObject<T>(node[nameof(Perfect)]);
            LatePerfect = (T)ModelUtils.ToObject<T>(node[nameof(LatePerfect)]);
            VeryLate = (T)ModelUtils.ToObject<T>(node[nameof(VeryLate)]);
            TooLate = (T)ModelUtils.ToObject<T>(node[nameof(TooLate)]);
            Multipress = (T)ModelUtils.ToObject<T>(node[nameof(Multipress)]);
            FailMiss = (T)ModelUtils.ToObject<T>(node[nameof(FailMiss)]);
            FailOverload = (T)ModelUtils.ToObject<T>(node[nameof(FailOverload)]);
        }
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
    public class JudgeM<T> : Judge<T>, IModel where T : IModel, ICopyable<T>, new()
    {
        public new JsonNode Serialize()
        {
            JsonNode node = JsonNode.Empty;
            node[nameof(TooEarly)] = TooEarly.Serialize();
            node[nameof(VeryEarly)] = VeryEarly.Serialize();
            node[nameof(EarlyPerfect)] = EarlyPerfect.Serialize();
            node[nameof(Perfect)] = Perfect.Serialize();
            node[nameof(LatePerfect)] = LatePerfect.Serialize();
            node[nameof(VeryLate)] = VeryLate.Serialize();
            node[nameof(TooLate)] = TooLate.Serialize();
            node[nameof(Multipress)] = Multipress.Serialize();
            node[nameof(FailMiss)] = FailMiss.Serialize();
            node[nameof(FailOverload)] = FailOverload.Serialize();
            return node;
        }
        public new void Deserialize(JsonNode node)
        {
            TooEarly = ModelUtils.Unbox<T>(node[nameof(TooEarly)]);
            VeryEarly = ModelUtils.Unbox<T>(node[nameof(VeryEarly)]);
            EarlyPerfect = ModelUtils.Unbox<T>(node[nameof(EarlyPerfect)]);
            Perfect = ModelUtils.Unbox<T>(node[nameof(Perfect)]);
            LatePerfect = ModelUtils.Unbox<T>(node[nameof(LatePerfect)]);
            VeryLate = ModelUtils.Unbox<T>(node[nameof(VeryLate)]);
            TooLate = ModelUtils.Unbox<T>(node[nameof(TooLate)]);
            Multipress = ModelUtils.Unbox<T>(node[nameof(Multipress)]);
            FailMiss = ModelUtils.Unbox<T>(node[nameof(FailMiss)]);
            FailOverload = ModelUtils.Unbox<T>(node[nameof(FailOverload)]);
        }
        public new JudgeM<T> Copy()
        {
            var newJudge = new JudgeM<T>();
            newJudge.TooEarly = TooEarly.Copy();
            newJudge.VeryEarly = VeryEarly.Copy();
            newJudge.EarlyPerfect = EarlyPerfect.Copy();
            newJudge.Perfect = Perfect.Copy();
            newJudge.LatePerfect = LatePerfect.Copy();
            newJudge.VeryLate = VeryLate.Copy();
            newJudge.TooLate = TooLate.Copy();
            newJudge.Multipress = Multipress.Copy();
            newJudge.FailMiss = FailMiss.Copy();
            newJudge.FailOverload = FailOverload.Copy();
            return newJudge;
        }
    }
}