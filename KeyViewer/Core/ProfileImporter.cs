using KeyViewer.Models;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KeyViewer.Core;

public static class ProfileImporter {
    public static Profile Import(JToken node) {
        var profile = ModelUtils.Unbox<Profile>(node);

        var refsToken = node["References"];
        var refs = refsToken is JArray arr
            ? ModelUtils.UnwrapList<FileReference>(arr)
            : [];

        if(refs.Any()) {
            var refsDir = Path.Combine(Main.ProfilePath, "References");
            var fontsDir = Path.Combine(refsDir, "Fonts");
            var imagesDir = Path.Combine(refsDir, "Images");

            Directory.CreateDirectory(refsDir);
            if(refs.Any(r => r.ReferenceType == FileReference.Type.Font)) {
                Directory.CreateDirectory(fontsDir);
            }
            if(refs.Any(r => r.ReferenceType == FileReference.Type.Image)) {
                Directory.CreateDirectory(imagesDir);
            }

            foreach(var r in refs) {
                if(r.ReferenceType == FileReference.Type.Font) {
                    var targetPath = Path.Combine(fontsDir, r.Name);
                    File.WriteAllBytes(targetPath, r.Raw);

                    foreach(var key in profile.Keys) {
                        var f = key.Font?.Replace("{ModDir}", Main.Mod.Path);
                        if(Path.GetFileName(f ?? "") == r.Name) {
                            key.Font = targetPath;
                        }
                    }
                } else if(r.ReferenceType == FileReference.Type.Image) {
                    var targetPath = Path.Combine(imagesDir, r.Name);
                    File.WriteAllBytes(targetPath, r.Raw);

                    foreach(var img in profile.Keys.SelectMany(k => new[] { k.Background, k.Outline })) {
                        var p = img.Pressed?.Replace("{ModDir}", Main.Mod.Path);
                        var q = img.Released?.Replace("{ModDir}", Main.Mod.Path);

                        if(Path.GetFileName(p ?? "") == r.Name) {
                            img.Pressed = targetPath;
                        }
                        if(Path.GetFileName(q ?? "") == r.Name) {
                            img.Released = targetPath;
                        }
                    }
                }
            }
        }

        return profile;
    }

    public static JArray GetReferencesAsJson(Profile profile) => ModelUtils.WrapCollection(GetReferences(profile));

    public static List<FileReference> GetReferences(Profile profile) {
        List<FileReference> references = [];

        foreach(var key in profile.Keys) {
            if(!string.IsNullOrWhiteSpace(key.Font)) {
                references.Add(IOUtils.GetReference(key.Font, FileReference.Type.Font));
            }
        }

        foreach(var img in profile.Keys.SelectMany(k => new[] { k.Background, k.Outline })) {
            if(!string.IsNullOrWhiteSpace(img.Pressed)) {
                references.Add(IOUtils.GetReference(img.Pressed, FileReference.Type.Image));
            }
            if(!string.IsNullOrWhiteSpace(img.Released)) {
                references.Add(IOUtils.GetReference(img.Released, FileReference.Type.Image));
            }
        }

        return references.Where(r => r != null).Distinct().ToList();
    }
}