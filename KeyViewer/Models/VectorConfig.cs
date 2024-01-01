using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using UnityEngine;

namespace KeyViewer.Models
{
    public class VectorConfig : IModel
    {
        public PressRelease<Vector3> Rotation = Vector3.zero;
        public PressRelease<Vector2> Offset = Vector2.zero;
        public PressRelease<Vector2> Scale = Vector2.one;
        public float Size { get => Scale.Pressed.x; set => Scale.Pressed.x = value; }
        public VectorConfig Copy()
        {
            VectorConfig newRos = new VectorConfig();
            newRos.Rotation = Rotation.Copy();
            newRos.Offset = Offset.Copy();
            newRos.Scale = Scale.Copy();    
            return newRos;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Rotation)] = Rotation.Serialize();
            node[nameof(Offset)] = Offset.Serialize();
            node[nameof(Scale)] = Scale.Serialize();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Rotation = ModelUtils.Unbox<PressRelease<Vector3>>(node[nameof(Rotation)]);
            Offset = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Offset)]);
            Scale = ModelUtils.Unbox<PressRelease<Vector2>>(node[nameof(Scale)]);
        }
    }
}
