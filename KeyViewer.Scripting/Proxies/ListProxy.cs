using JSNet.API;

namespace KeyViewer.Scripting.Proxies
{
    [Api("List")]
    public class ListProxy
    {
        public int Count;
        public void Add(object obj) { }
        public bool Contains(object obj) { return false; }
        public void Remove(object obj) { }
        public void Clear() { }
        public ListProxy GetRange(int start, int count) { return null; }
        public int IndexOf(object obj) { return 0; }
        public int IndexOf(object obj, int index) { return 0; }
        public int IndexOf(object obj, int index, int count) { return 0; }
        public int LastIndexOf(object obj) { return 0; }
        public int LastIndexOf(object obj, int index) { return 0; }
        public int LastIndexOf(object obj, int index, int count) { return 0; }
        public void Insert(int index, object obj) { }
        public void RemoveAt(int index) { }
        public void Reverse() { }
        public void Reverse(int index, int count) { }
        public void Sort() { }
        public void TrimExcess() { }
        public object[] ToArray() { return null; }
    }
}
