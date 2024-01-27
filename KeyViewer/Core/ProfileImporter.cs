using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KeyViewer.Core
{
    public static class ProfileImporter
    {
        public static Profile Import(JsonNode node)
        {
            var profile = ModelUtils.Unbox<Profile>(node);
            var refsNode = (JsonArray)node["References"].IfNotExist(new JsonArray());
            var refs = ModelUtils.UnwrapList<Reference>(refsNode);
            if (refs.Any())
            {
                var refsDir = Path.Combine(Main.Mod.Path, "References");
                var fontsDir = Path.Combine(refsDir, "Fonts");
                var imagesDir = Path.Combine(refsDir, "Images");
                Directory.CreateDirectory(refsDir);
                if (refs.Any(r => r.ReferenceType == Reference.Type.Font))
                    Directory.CreateDirectory(fontsDir);
                if (refs.Any(r => r.ReferenceType == Reference.Type.Image))
                    Directory.CreateDirectory(imagesDir);
                foreach (var @ref in refs)
                {
                    if (@ref.ReferenceType == Reference.Type.Font)
                    {
                        var targetPath = Path.Combine(fontsDir, @ref.Name);
                        File.WriteAllBytes(targetPath, @ref.Raw.Decompress());
                        foreach (var text in profile.Keys)
                            if ((Path.GetFileName(text.Font?.Replace("{ModDir}", Main.Mod.Path)) ?? "") == @ref.Name)
                                text.Font = targetPath;
                    }
                    else if (@ref.ReferenceType == Reference.Type.Image)
                    {
                        var targetPath = Path.Combine(imagesDir, @ref.Name);
                        File.WriteAllBytes(targetPath, @ref.Raw.Decompress());
                        foreach (var text in profile.Keys.SelectMany(k => new[] { k.Background, k.Outline }))
                        {
                            if ((Path.GetFileName(text.Pressed?.Replace("{ModDir}", Main.Mod.Path)) ?? "") == @ref.Name)
                                text.Pressed = targetPath;
                            if ((Path.GetFileName(text.Released?.Replace("{ModDir}", Main.Mod.Path)) ?? "") == @ref.Name)
                                text.Released = targetPath;
                        }
                    }
                }
            }
            return profile;
        }
        public static JsonArray GetReferences(Profile profile)
        {
            List<Reference> references = new List<Reference>();
            foreach (var text in profile.Keys)
                if (!string.IsNullOrWhiteSpace(text.Font))
                    references.Add(Reference.GetReference(text.Font, Reference.Type.Font));
            foreach (var text in profile.Keys.SelectMany(k => new[] { k.Background, k.Outline }))
            {
                if (!string.IsNullOrWhiteSpace(text.Pressed))
                    references.Add(Reference.GetReference(text.Pressed, Reference.Type.Image));
                if (!string.IsNullOrWhiteSpace(text.Released))
                    references.Add(Reference.GetReference(text.Released, Reference.Type.Image));
            }
            return ModelUtils.WrapList(references.Where(r => r != null).Distinct().ToList());
        }
        public class Reference : IModel, ICopyable<Reference>
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
            static Dictionary<string, Reference> refCache = new Dictionary<string, Reference>();
            public static Reference GetReference(string path, Type referenceType)
            {
                var target = path.Replace("{ModDir}", Main.Mod.Path);
                if (refCache.TryGetValue(target, out var reference)) return reference;
                var @ref = new Reference();
                @ref.From = target;
                @ref.Name = Path.GetFileName(target);
                @ref.ReferenceType = referenceType;
                if (File.Exists(target))
                {
                    @ref.Raw = File.ReadAllBytes(target).Compress();
                    return refCache[target] = @ref;
                }
                return null;
            }
            public JsonNode Serialize()
            {
                var node = JsonNode.Empty;
                node[nameof(ReferenceType)] = ReferenceType.ToString();
                node[nameof(From)] = From;
                node[nameof(Name)] = Name;
                node[nameof(Raw)] = Raw;
                node[nameof(Raw)].Inline = true;
                return node;
            }
            public void Deserialize(JsonNode node)
            {
                ReferenceType = EnumHelper<Type>.Parse(node[nameof(ReferenceType)]);
                From = node[nameof(From)];
                Name = node[nameof(Name)];
                Raw = node[nameof(Raw)];
            }
            public Reference Copy()
            {
                var newRef = new Reference();
                newRef.ReferenceType = ReferenceType;
                newRef.From = From;
                newRef.Name = Name;
                newRef.Raw = Raw;
                return newRef;
            }
        }
    }
}
