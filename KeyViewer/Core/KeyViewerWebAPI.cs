using JSON;
using KeyViewer.Models;
using KeyViewer.Utils;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KeyViewer.Core
{
    public static class KeyViewerWebAPI
    {
        public const string API = DEV_API;
        public const string DEV_API = "http://localhost:1111";
        public const string PROD_API = "https://api.keyviewer.net";
        public static async Task<string> Handshake() => await Main.HttpClient.GetStringAsync(API + "/handshake");
        public static async Task<Version> GetVersion() => Version.Parse(JsonNode.Parse(await Main.HttpClient.GetStringAsync(API + "/version")).Value);
        public static async Task<string> GetDiscordLink() => await Main.HttpClient.GetStringAsync(API + "/discord");
        public static async Task<string> GetDownloadLink() => await Main.HttpClient.GetStringAsync(API + "/download");
        public static async Task<string> GetLanguageJson(KeyViewerLanguage lang) => await Main.HttpClient.GetStringAsync(API + "/language/" + lang);
        public static async Task<byte[]> EncryptProfile(Profile profile, Metadata metadata, string key)
        {
            return EncryptedProfileHelper.Encrypt(profile, key, metadata);
            //try
            //{
                var profileJson = profile.Serialize();
                profileJson.Inline = true;
                profileJson["References"] = ProfileImporter.GetReferencesAsJson(profile);
                var emptyJson = JsonNode.Empty;
                emptyJson.Inline = true;
                emptyJson["Profile"] = profileJson;
                emptyJson["Metadata"] = metadata.Serialize();
                var array = new ByteArrayContent(Encoding.UTF8.GetBytes(emptyJson.ToString()));
                array.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
                var response = await Main.HttpClient.PostAsync(API + $"/encryptedprofile/encrypt/{key}", array);
                response.EnsureSuccessStatusCode();
                var encryptedProfile = await response.Content.ReadAsByteArrayAsync();
                return encryptedProfile;
            //}
            //catch { return null; }
        }
        public static async Task<EncryptedProfile> OpenEncryptedProfile(byte[] encryptedProfile)
        {
            return EncryptedProfileHelper.Open(encryptedProfile);
            //try
            //{
            var array = new ByteArrayContent(encryptedProfile);
                array.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
                var response = await Main.HttpClient.PostAsync(API + "/encryptedprofile/open", array);
                response.EnsureSuccessStatusCode();
                var openedProfile = await response.Content.ReadAsByteArrayAsync();
                var json = JsonNode.Parse(Encoding.UTF8.GetString(openedProfile));
                Main.Logger.Log(Encoding.UTF8.GetString(openedProfile));
                return ModelUtils.Unbox<EncryptedProfile>(json);
            //}
            //catch { return null; }
        }
        public static async Task<Profile> DecryptProfile(byte[] rawProfile, string key)
        {
            return EncryptedProfileHelper.DecryptRaw(rawProfile, key);
            //try
            //{
            var array = new ByteArrayContent(rawProfile);
                array.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
                var response = await Main.HttpClient.PostAsync(API + $"/encryptedprofile/decrypt/{key}", array);
                response.EnsureSuccessStatusCode();
                var openedProfile = await response.Content.ReadAsByteArrayAsync();
                var json = JsonNode.Parse(Encoding.UTF8.GetString(openedProfile));
                return ModelUtils.Unbox<Profile>(json);
            //}
            //catch { return null; }
        }
    }
}
