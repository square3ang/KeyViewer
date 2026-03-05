using KeyViewer.Core.Interfaces;
using Newtonsoft.Json.Linq;
using System;

namespace KeyViewer.Models;

public class Metadata : IModel, ICopyable<Metadata> {
    public string Name = null;
    public string Author = null;
    public string Description = null;
    public long CreationTick = DateTime.Now.Ticks;
    public Metadata Copy() {
        var data = new Metadata {
            Name = Name,
            Author = Author,
            Description = Description,
            CreationTick = CreationTick
        };
        return data;
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(Name)] = Name,
            [nameof(Author)] = Author,
            [nameof(Description)] = Description,
            [nameof(CreationTick)] = CreationTick
        };
        return node;
    }
    public void Deserialize(JToken node) {
        var defaultSettings = new Metadata();

        Name = node[nameof(Name)]?.Value<string>() ?? defaultSettings.Name;
        Author = node[nameof(Author)]?.Value<string>() ?? defaultSettings.Author;
        Description = node[nameof(Description)]?.Value<string>() ?? defaultSettings.Description;
        CreationTick = node[nameof(CreationTick)]?.Value<long>() ?? defaultSettings.CreationTick;
    }
}
