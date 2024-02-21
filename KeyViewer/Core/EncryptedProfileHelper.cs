using JSON;
using KeyViewer.Models;
using KeyViewer.Utils;
using System.Text;

namespace KeyViewer.Core
{
    public static class EncryptedProfileHelper
    {
        public static byte[] Encrypt(Profile profile, string key, Metadata metadata)
        {
            try
            {
                var profileJsonNode = profile.Serialize();
                profileJsonNode.Inline = true;
                var profileJson = profileJsonNode.ToString().Trim();
                var profileJsonEncrypted = CryptoUtils.Xor(profileJson, key);
                var eProfile = new EncryptedProfile() { Metadata = metadata };
                eProfile.RawProfile = Encoding.UTF8.GetBytes(profileJsonEncrypted);
                var encProfileJsonNode = eProfile.Serialize();
                encProfileJsonNode.Inline = true;
                var encProfileJson = encProfileJsonNode.ToString().Trim();
                return CryptoUtils.EncryptAes(encProfileJson, CryptoUtils.DefaultKey);
            }
            catch { return null; }
        }
        public static JsonNode OpenAsJson(byte[] encryptedProfile)
        {
            try
            {
                var encProfileJson = CryptoUtils.DecryptAes(encryptedProfile, CryptoUtils.DefaultKey);
                return JsonNode.Parse(encProfileJson);
            }
            catch { return null; }
        }
        public static EncryptedProfile Open(byte[] encryptedProfile)
        {
            try
            {
                var encProfileJsonNode = OpenAsJson(encryptedProfile);
                return ModelUtils.Unbox<EncryptedProfile>(encProfileJsonNode);
            }
            catch { return null; }
        }
        public static JsonNode DecryptAsJson(byte[] encryptedProfile, string key)
        {
            try
            {
                var eProfile = Open(encryptedProfile);
                var profileJsonEncrypted = Encoding.UTF8.GetString(eProfile!.RawProfile);
                var profileJson = CryptoUtils.Xor(profileJsonEncrypted, key);
                return JsonNode.Parse(profileJson);
            }
            catch { return null; }
        }
        public static JsonNode Decrypt(byte[] encryptedProfile, string key)
        {
            try
            {
                var profileJson = DecryptAsJson(encryptedProfile, key);
                return JsonNode.Parse(profileJson);
            }
            catch { return null; }
        }
        public static Profile DecryptRaw(byte[] rawProfile, string key)
        {
            try
            {
                var profileJsonEncrypted = Encoding.UTF8.GetString(rawProfile);
                var profileJson = CryptoUtils.Xor(profileJsonEncrypted, key);
                var profileJsonNode = JsonNode.Parse(profileJson);
                return ModelUtils.Unbox<Profile>(profileJsonNode);
            }
            catch { return null; }
        }
    }
}
