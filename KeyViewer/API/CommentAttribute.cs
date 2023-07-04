using System;

namespace KeyViewer.API
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CommentAttribute : Attribute
    {
        public CommentAttribute(string comment) => Comment = comment;
        public string Comment { get; }
    }
}
