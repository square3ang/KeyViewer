using JSON;
using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Utils;
using KeyViewer.WebAPI.Core.Utils;
using System.Text;

namespace KeyViewer.WebAPI.Core
{
    public static class EncryptedProfileHelper
    {
        public static byte[]? Encrypt(Profile profile, string key, Metadata metadata, List<FileReference> refs)
        {
            try
            {
                var profileJsonNode = profile.Serialize();
                profileJsonNode["References"] = ModelUtils.WrapCollection(refs);
                profileJsonNode.Inline = true;
                var profileJson = profileJsonNode.ToString().Trim();
                var profileJsonEncrypted = CryptoUtils.Xor(profileJson, key);
                var eProfile = new EncryptedProfile() { Metadata = metadata };
                eProfile.RawProfile = Encoding.UTF8.GetBytes(profileJsonEncrypted);
                var encProfileJsonNode = eProfile.Serialize();
                encProfileJsonNode.Inline = true;
                var encProfileJson = encProfileJsonNode.ToString().Trim();
                var aes = CryptoUtils.EncryptAes(encProfileJson, CryptoUtils.DefaultKey1).Compress();
                return CryptoUtils.Xor(aes, CryptoUtils.DefaultKey2Bytes);
            }
#if !DEBUG
            catch { return null; }
#else
            catch (Exception e) { Console.WriteLine($"Error On 'Encrypt'!\n{e}"); return null; }
#endif
        }
        public static JsonNode? OpenAsJson(byte[] encryptedProfile)
        {
            try
            {
                var xorDecrypt = CryptoUtils.Xor(encryptedProfile, CryptoUtils.DefaultKey2Bytes);
                var encProfileJson = CryptoUtils.DecryptAes(xorDecrypt.Decompress(), CryptoUtils.DefaultKey1);
                return JsonNode.Parse(encProfileJson);
            }
#if !DEBUG
            catch { return null; }
#else
            catch (Exception e) { Console.WriteLine($"Error On 'OpenAsJson'!\n{e}"); return null; }
#endif
        }
        public static EncryptedProfile? Open(byte[] encryptedProfile)
        {
            try
            {
                var encProfileJsonNode = OpenAsJson(encryptedProfile);
                return ModelUtils.Unbox<EncryptedProfile>(encProfileJsonNode);
            }
#if !DEBUG
            catch { return null; }
#else
            catch (Exception e) { Console.WriteLine($"Error On 'Open'!\n{e}"); return null; }
#endif
        }
        public static JsonNode? DecryptAsJson(byte[] encryptedProfile, string key)
        {
            try
            {
                var eProfile = Open(encryptedProfile);
                var profileJsonEncrypted = Encoding.UTF8.GetString(eProfile!.RawProfile);
                var profileJson = CryptoUtils.Xor(profileJsonEncrypted, key);
                return JsonNode.Parse(profileJson);
            }
#if !DEBUG
            catch { return null; }
#else
            catch (Exception e) { Console.WriteLine($"Error On 'DecryptAsJson'!\n{e}"); return null; }
#endif
        }
        public static Profile? Decrypt(byte[] encryptedProfile, string key)
        {
            try
            {
                var profileJson = DecryptAsJson(encryptedProfile, key);
                return ModelUtils.Unbox<Profile>(profileJson);
            }
#if !DEBUG
            catch { return null; }
#else
            catch (Exception e) { Console.WriteLine($"Error On 'Decrypt'!\n{e}"); return null; }
#endif
        }
        public static JsonNode? DecryptRawAsJson(byte[] rawProfile, string key)
        {
            try
            {
                var profileJsonEncrypted = Encoding.UTF8.GetString(rawProfile);
                var profileJson = CryptoUtils.Xor(profileJsonEncrypted, key);
                var profileJsonNode = JsonNode.Parse(profileJson);
                return profileJsonNode;
            }
#if !DEBUG
            catch { return null; }
#else
            catch (Exception e) { Console.WriteLine($"Error On 'DecryptRawAsJson'!\n{e}"); return null; }
#endif
        }
        public static Profile? DecryptRaw(byte[] rawProfile, string key)
        {
            try
            {
                var profileJsonNode = DecryptRawAsJson(rawProfile, key);
                return ModelUtils.Unbox<Profile>(profileJsonNode);
            }
#if !DEBUG
            catch { return null; }
#else
            catch (Exception e) { Console.WriteLine($"Error On 'DecryptRaw'!\n{e}"); return null; }
#endif
        }
    }
}
