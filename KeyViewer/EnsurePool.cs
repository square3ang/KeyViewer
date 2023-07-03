using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace KeyViewer
{
    public class EnsurePool<T>
    {
        private List<T> pool;
        private Func<T> ensurer;
        private Predicate<T> criteria;
        private Action<T> onGet;
        private Action<T> onClear;
        public EnsurePool(Func<T> ensurer, Predicate<T> ensureCriteria, Action<T> onGet = null, Action<T> onClear = null, int capacity = -1) 
        {
            if (ensurer == null)
                throw new ArgumentNullException(nameof(ensurer), "Ensurer Cannot Be Null!");
            if (ensureCriteria == null)
                throw new ArgumentNullException(nameof(ensureCriteria), "Ensure Criteria Cannot Be Null!");
            pool = new List<T>();
            this.ensurer = ensurer;
            criteria = ensureCriteria;
            this.onGet = onGet;
            this.onClear = onClear;
            if (capacity > 0)
                Fill(capacity);
        }
        [NotNull]
        public T Get()
        {
            foreach (T t in pool)
                if (criteria(t))
                {
                    onGet?.Invoke(t);
                    return t;
                }
            T ensured = ensurer();
            pool.Add(ensured);
            onGet?.Invoke(ensured);
            return ensured;
        }
        public void Clear()
        {
            ForEach(onClear);
            pool.Clear();
        }
        public void Fill(int count)
        {
            for (int i = 0; i < count; i++)
                pool.Add(ensurer());
        }
        public int Count => pool.Count;
        public void ForEach(Action<T> forEach)
        {
            if (forEach == null) return;
            foreach (T t in pool)
                forEach(t);
        }
    }
}
