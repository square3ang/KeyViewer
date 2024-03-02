using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Utils;
using System;
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
            var refs = ModelUtils.UnwrapList<FileReference>(refsNode);
            if (refs.Any())
            {
                var refsDir = Path.Combine(Main.Mod.Path, "References");
                var fontsDir = Path.Combine(refsDir, "Fonts");
                var imagesDir = Path.Combine(refsDir, "Images");
                Directory.CreateDirectory(refsDir);
                if (refs.Any(r => r.ReferenceType == FileReference.Type.Font))
                    Directory.CreateDirectory(fontsDir);
                if (refs.Any(r => r.ReferenceType == FileReference.Type.Image))
                    Directory.CreateDirectory(imagesDir);
                foreach (var @ref in refs)
                {
                    if (@ref.ReferenceType == FileReference.Type.Font)
                    {
                        var targetPath = Path.Combine(fontsDir, @ref.Name);
                        File.WriteAllBytes(targetPath, @ref.Raw);
                        foreach (var text in profile.Keys)
                            if ((Path.GetFileName(text.Font?.Replace("{ModDir}", Main.Mod.Path)) ?? "") == @ref.Name)
                                text.Font = targetPath;
                    }
                    else if (@ref.ReferenceType == FileReference.Type.Image)
                    {
                        var targetPath = Path.Combine(imagesDir, @ref.Name);
                        File.WriteAllBytes(targetPath, @ref.Raw);
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
        public static JsonArray GetReferencesAsJson(Profile profile)
        {
            return ModelUtils.WrapCollection(GetReferences(profile));
        }
        public static List<FileReference> GetReferences(Profile profile)
        {
            List<FileReference> references = new List<FileReference>();
            foreach (var text in profile.Keys)
                if (!string.IsNullOrWhiteSpace(text.Font))
                    references.Add(IOUtils.GetReference(text.Font, FileReference.Type.Font));
            foreach (var text in profile.Keys.SelectMany(k => new[] { k.Background, k.Outline }))
            {
                if (!string.IsNullOrWhiteSpace(text.Pressed))
                    references.Add(IOUtils.GetReference(text.Pressed, FileReference.Type.Image));
                if (!string.IsNullOrWhiteSpace(text.Released))
                    references.Add(IOUtils.GetReference(text.Released, FileReference.Type.Image));
            }
            return references.Where(r => r != null).Distinct().ToList();
        }
    }
}
