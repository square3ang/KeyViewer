﻿using JSON;
using KeyViewer.Core.Interfaces;

namespace KeyViewer.Models
{
    public struct RainImage : IModel
    {
        public int Count;
        public string Image;
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Count)] = Count;
            node[nameof(Image)] = Image;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Count = node[nameof(Count)];
            var img = node[nameof(Image)];
            if (img == null)
                Image = null;
            else Image = img.Value;
        }
    }
}
