using KeyViewer.Core.Interfaces;
using KeyViewer.Unity;
using System;
using UnityEngine;

namespace KeyViewer.Core
{
    public abstract class ModelDrawable<T> : IDrawable where T : IModel
    {
        public T model;
        public ModelDrawable(T model)
        {
            this.model = model;
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
