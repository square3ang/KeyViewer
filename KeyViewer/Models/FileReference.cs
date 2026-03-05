using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace KeyViewer.Models;

public class FileReference : IModel, ICopyable<FileReference> {
    public enum Type {
        Font,
        Image,
    }
    public Type ReferenceType;
    public string From;
    public string Name;
    public byte[] Raw;
    public JToken Serialize() {
        var node = new JObject {
            [nameof(ReferenceType)] = ReferenceType.ToString(),
            [nameof(From)] = From,
            [nameof(Name)] = Name,
            [nameof(Raw)] = Convert.ToBase64String(Raw.Compress())
        };
        return node;
    }
    public void Deserialize(JToken node) {
        ReferenceType = EnumHelper<Type>.Parse(node[nameof(ReferenceType)]?.Value<string>() ?? "");

        From = node[nameof(From)]?.Value<string>() ?? "";
        Name = node[nameof(Name)]?.Value<string>() ?? "";

        var rawNode = node[nameof(Raw)];
        if(rawNode == null) {
            Raw = Array.Empty<byte>();
            return;
        }

        Raw = rawNode.Type == JTokenType.Array
            ? rawNode.Values<byte>().ToArray().Decompress()
            : Convert.FromBase64String(rawNode.Value<string>()).Decompress();
    }
    public FileReference Copy() {
        var newRef = new FileReference {
            ReferenceType = ReferenceType,
            From = From,
            Name = Name,
            Raw = (byte[])Raw.Clone()
        };
        return newRef;
    }
}
