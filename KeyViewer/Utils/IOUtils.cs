using KeyViewer.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace KeyViewer.Utils;

public static class IOUtils {
    public static byte[] Zip(params RawFile[] rFiles) {
        using(MemoryStream ms = new()) {
            using(ZipArchive zipArchive = new(ms, ZipArchiveMode.Update, false, Encoding.UTF8)) {
                foreach(var rFile in rFiles) {
                    var entry = zipArchive.CreateEntry(rFile.Name);
                    using(Stream entryStream = entry.Open()) {
                        entryStream.Write(rFile.Data, 0, rFile.Data.Length);
                    }
                }
            }
            return ms.ToArray();
        }
    }
    public static byte[] ZipFiles(params string[] files) {
        using(MemoryStream ms = new())
        using(ZipArchive zipArchive = new(ms, ZipArchiveMode.Update, false, Encoding.UTF8)) {
            foreach(string file in files) {
                zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
            }

            return ms.ToArray();
        }
    }
    public static List<RawFile> Unzip(byte[] zip) {
        List<RawFile> files = [];
        using(MemoryStream ms = new(zip, 0, zip.Length, true, true))
        using(ZipArchive zipArchive = new(ms, ZipArchiveMode.Read, false, Encoding.UTF8)) {
            foreach(var entry in zipArchive.Entries) {
                using(Stream entryStream = entry.Open()) {
                    byte[] buffer = new byte[entryStream.Length];
                    entryStream.Read(buffer, 0, buffer.Length);
                    files.Add(new RawFile(entry.FullName, buffer));
                }
            }
        }
        return files;
    }
    public static void Unzip(string zipFile, string destDir) {
        if(!Directory.Exists(destDir)) {
            Directory.CreateDirectory(destDir);
        }

        ZipFile.ExtractToDirectory(zipFile, destDir);
    }
    static Dictionary<string, FileReference> refCache = [];
    public static FileReference GetReference(string path, FileReference.Type referenceType) {
        var target = path.Replace("{ModDir}", Main.Mod.Path);
        if(refCache.TryGetValue(target, out var reference)) {
            return reference;
        }

        var @ref = new FileReference {
            From = target,
            Name = Path.GetFileName(target),
            ReferenceType = referenceType
        };
        if(File.Exists(target)) {
            @ref.Raw = File.ReadAllBytes(target);
            return refCache[target] = @ref;
        }
        return null;
    }
}
public class RawFile {
    public string Name { get; }
    public byte[] Data { get; }
    public RawFile(string name, byte[] data) {
        Name = name;
        Data = data;
    }
}
