using JSON;
using KeyViewer.Core.Interfaces;
using System;

namespace KeyViewer.Models
{
    public class Metadata : IModel, ICopyable<Metadata>
    {
        public string Name;
        public string Author;
        public string Description;
        public DateTime CreationTime = DateTime.Now;
        public Metadata Copy()
        {
            var data = new Metadata();
            data.Name = Name;
            data.Author = Author;
            data.Description = Description;
            data.CreationTime = CreationTime;
            return data;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Name)] = Name;
            node[nameof(Author)] = Author;
            node[nameof(Description)] = Description;
            node[nameof(CreationTime)] = CreationTime;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Name = node[nameof(Name)];
            Author = node[nameof(Author)];
            Description = node[nameof(Description)];
            CreationTime = node[nameof(CreationTime)];
        }
    }
}
