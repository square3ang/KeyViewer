using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class Judge<T> : IModel
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
    public class JudgeM<T> : Judge<T>, IModel where T : IModel, new()
    {
        public new JsonNode Serialize()
        {
            JsonNode node = JsonNode.Empty;
            node[nameof(TooEarly)] = TooEarly.Serialize();
            node[nameof(VeryEarly)] = TooEarly.Serialize();
            node[nameof(EarlyPerfect)] = TooEarly.Serialize();
            node[nameof(Perfect)] = TooEarly.Serialize();
            node[nameof(LatePerfect)] = TooEarly.Serialize();
            node[nameof(VeryLate)] = TooEarly.Serialize();
            node[nameof(TooLate)] = TooEarly.Serialize();
            node[nameof(Multipress)] = TooEarly.Serialize();
            node[nameof(FailMiss)] = TooEarly.Serialize();
            node[nameof(FailOverload)] = TooEarly.Serialize();
            return node;
        }
        public new void Deserialize(JsonNode node)
        {
            T te = new T();
            te.Deserialize(node[nameof(TooEarly)]);
            TooEarly = te;

            T ve = new T();
            ve.Deserialize(node[nameof(VeryEarly)]);
            VeryEarly = ve;

            T ep = new T();
            ep.Deserialize(node[nameof(EarlyPerfect)]);
            EarlyPerfect = ep;

            T p = new T();
            p.Deserialize(node[nameof(Perfect)]);
            Perfect = p;

            T lp = new T();
            lp.Deserialize(node[nameof(LatePerfect)]);
            LatePerfect = lp;

            T vl = new T();
            vl.Deserialize(node[nameof(VeryLate)]);
            VeryLate = vl;

            T tl = new T();
            tl.Deserialize(node[nameof(TooLate)]);
            TooLate = tl;

            T mp = new T();
            mp.Deserialize(node[nameof(Multipress)]);
            Multipress = mp;

            T fm = new T();
            fm.Deserialize(node[nameof(FailMiss)]);
            FailMiss = fm;

            T fo = new T();
            fo.Deserialize(node[nameof(FailOverload)]);
            FailOverload = fo;
        }
        public new JudgeM<T> Copy()
        {
            var newJudge = new JudgeM<T>();
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