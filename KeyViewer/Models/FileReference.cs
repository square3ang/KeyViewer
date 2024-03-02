using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using System;

namespace KeyViewer.Models
{
    public class FileReference : IModel, ICopyable<FileReference>
    {
        public enum Type
        {
            Font,
            Image,
        }
        public Type ReferenceType;
        public string From;
        public string Name;
        public byte[] Raw;
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(ReferenceType)] = ReferenceType.ToString();
            node[nameof(From)] = From;
            node[nameof(Name)] = Name;
            node[nameof(Raw)] = Convert.ToBase64String(Raw.Compress());
            node[nameof(Raw)].Inline = true;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            ReferenceType = EnumHelper<Type>.Parse(node[nameof(ReferenceType)]);
            From = node[nameof(From)];
            Name = node[nameof(Name)];
            var rawNode = node[nameof(Raw)];
            if (rawNode.IsArray) Raw = ((byte[])rawNode).Decompress();
            else Raw = Convert.FromBase64String(rawNode.Value).Decompress();
        }
        public FileReference Copy()
        {
            var newRef = new FileReference();
            newRef.ReferenceType = ReferenceType;
            newRef.From = From;
            newRef.Name = Name;
            newRef.Raw = (byte[])Raw.Clone();
            return newRef;
        }
    }
}
