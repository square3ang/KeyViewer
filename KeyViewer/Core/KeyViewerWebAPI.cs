using JSON;
using KeyViewer.Models;
using System;
using System.Threading.Tasks;

namespace KeyViewer.Core
{
    public static class KeyViewerWebAPI
    {
        public const string PROD_API = "https://api.keyviewer.net";
        public const string DEV_API = "http://localhost:1111";
        public static async Task<string> Handshake() => await Main.HttpClient.GetStringAsync(PROD_API + "/handshake");
        public static async Task<Version> GetVersion() => Version.Parse(JsonNode.Parse(await Main.HttpClient.GetStringAsync(PROD_API + "/version")).Value);
        public static async Task<string> GetDiscordLink() => await Main.HttpClient.GetStringAsync(PROD_API + "/discord");
        public static async Task<string> GetDownloadLink() => await Main.HttpClient.GetStringAsync(PROD_API + "/download");
        public static async Task<string> GetLanguageJson(KeyViewerLanguage lang) => await Main.HttpClient.GetStringAsync(PROD_API + "/language/" + lang);
    }
}
