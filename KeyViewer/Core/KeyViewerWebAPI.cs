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
        public const string API = PROD_API;
        public const string DEV_API = "http://localhost:1111";
        public const string PROD_API = "https://api.keyviewer.net";
        public static async Task<string> Handshake() => await Main.HttpClient.GetStringAsync(API + "/handshake");
        public static async Task<Version> GetVersion() => Version.Parse(JsonNode.Parse(await Main.HttpClient.GetStringAsync(API + "/version")).Value);
        public static async Task<string> GetDiscordLink() => await Main.HttpClient.GetStringAsync(API + "/discord");
        public static async Task<string> GetDownloadLink() => await Main.HttpClient.GetStringAsync(API + "/download");
        public static async Task<string> GetLanguageJson(KeyViewerLanguage lang) => await Main.HttpClient.GetStringAsync(API + "/language/" + lang);
        public static async Task<byte[]> EncryptProfile(Profile profile, Metadata metadata, string key)
        {
            Main.Logger.Log($"Requesting Encrypt..");
            var profileJson = profile.Serialize();
            profileJson.Inline = true;
            profileJson["References"] = ProfileImporter.GetReferencesAsJson(profile);
            var emptyJson = JsonNode.Empty;
            emptyJson.Inline = true;
            emptyJson["Profile"] = profileJson;
            emptyJson["Metadata"] = metadata.Serialize();
            Main.Logger.Log($"Transmitted Hash: {Encoding.UTF8.GetBytes(emptyJson.ToString()).GetHashSHA1()} (Length:{Encoding.UTF8.GetBytes(emptyJson.ToString()).Length})");
            var array = new ByteArrayContent(Encoding.UTF8.GetBytes(emptyJson.ToString()));
            array.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
            var response = await Main.HttpClient.PostAsync(API + $"/encryptedprofile/encrypt/{key}", array);
            response.EnsureSuccessStatusCode();
            var encryptedProfile = await response.Content.ReadAsByteArrayAsync();
            Main.Logger.Log($"Received Hash: {encryptedProfile.GetHashSHA1()} (Length:{encryptedProfile.Length})");
            return encryptedProfile;
        }
        public static async Task<EncryptedProfile> OpenEncryptedProfile(byte[] encryptedProfile)
        {
            Main.Logger.Log($"Requesting Open..");
            Main.Logger.Log($"Transmitted Hash: {encryptedProfile.GetHashSHA1()} (Length:{encryptedProfile.Length})");
            var array = new ByteArrayContent(encryptedProfile);
            array.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
            var response = await Main.HttpClient.PostAsync(API + "/encryptedprofile/open", array);
            response.EnsureSuccessStatusCode();
            var openedProfile = await response.Content.ReadAsByteArrayAsync();
            Main.Logger.Log($"Received Hash: {openedProfile.GetHashSHA1()} (Length:{openedProfile.Length})");
            var json = JsonNode.Parse(Encoding.UTF8.GetString(openedProfile));
            return ModelUtils.Unbox<EncryptedProfile>(json);
        }
        public static async Task<Profile> DecryptProfile(byte[] rawProfile, string key)
        {
            Main.Logger.Log($"Requesting Open..");
            Main.Logger.Log($"Transmitted Hash: {rawProfile.GetHashSHA1()} (Length:{rawProfile.Length})");
            var array = new ByteArrayContent(rawProfile);
            array.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
            var response = await Main.HttpClient.PostAsync(API + $"/encryptedprofile/decrypt/{key}", array);
            response.EnsureSuccessStatusCode();
            var openedProfile = await response.Content.ReadAsByteArrayAsync();
            Main.Logger.Log($"Received Hash: {openedProfile.GetHashSHA1()} (Length:{openedProfile.Length})");
            var json = JsonNode.Parse(Encoding.UTF8.GetString(openedProfile));
            return ProfileImporter.Import(json);
        }
    }
}
