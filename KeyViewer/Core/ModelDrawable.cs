using KeyViewer.Core.Interfaces;
using UnityEngine;

namespace KeyViewer.Core
{
    public abstract class ModelDrawable<T> : IDrawable where T : IModel
    {
        public T model;
        public string Name { get; protected set; }
        public ModelDrawable(T model, string name)
        {
            this.model = model;
            Name = name;
        }
        public abstract void Draw();
        public virtual void OnKeyDown(KeyCode code) { }
        protected static string L(string translationKey, params object[] formatArgs)
        {
            if (formatArgs.Length == 0)
                return Main.Lang[translationKey];
            else return string.Format(Main.Lang[translationKey], formatArgs);
        }
    }
}
