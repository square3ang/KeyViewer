using JSON;
using KeyViewer.Models;
using KeyViewer.Utils;
using KeyViewer.WebAPI.Core;
using KeyViewer.WebAPI.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace KeyViewer.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EncryptedProfileController : ControllerBase
    {
        [HttpPost("open")]
        public async Task Open([FromBody] byte[] encryptedProfile)
        {
            DateWriteLine($"Open Requested From {HttpContext.GetIpAddress()}");
            DateWriteLine($"Received Hash: {encryptedProfile.GetHashSHA1()} (Length:{encryptedProfile.Length})");
            var json = EncryptedProfileHelper.OpenAsJson(encryptedProfile)?.ToString();
            if (json == null)
            {
                DateWriteLine($"Transmitted Hash: {Array.Empty<byte>().GetHashSHA1()} (Length:0)");
                return;
            }
            var result = Encoding.UTF8.GetBytes(json);
            DateWriteLine($"Transmitted Hash: {result.GetHashSHA1()} (Length:{result.Length})");
            await Response.Body.WriteAsync(result, 0, result.Length);
        }
        [HttpPost("encrypt/{key}")]
        public async Task Encrypt([FromBody] byte[] json, string key)
        {
            DateWriteLine($"Encrypt Requested From {HttpContext.GetIpAddress()}");
            DateWriteLine($"Received Hash: {json.GetHashSHA1()} (Length:{json.Length})");
            var strJson = Encoding.UTF8.GetString(json);
            var node = JsonNode.Parse(strJson);
            var metadata = ModelUtils.Unbox<Metadata>(node["Metadata"]);
            var profileNode = node["Profile"];
            var references = ModelUtils.UnwrapList<FileReference>((JsonArray)profileNode["References"]);
            var profile = ModelUtils.Unbox<Profile>(profileNode);
            var result = EncryptedProfileHelper.Encrypt(profile, key, metadata, references);
            if (result == null)
            {
                DateWriteLine($"Transmitted Hash: {Array.Empty<byte>().GetHashSHA1()} (Length:0)");
                return;
            }
            DateWriteLine($"Transmitted Hash: {result.GetHashSHA1()} (Length:{result.Length})");
            await Response.Body.WriteAsync(result, 0, result.Length);
        }
        [HttpPost("decrypt/{key}")]
        public async Task DecryptMeta([FromBody] byte[] encryptedProfile, string key)
        {
            DateWriteLine($"Decrypt Requested From {HttpContext.GetIpAddress()}");
            DateWriteLine($"Received Hash: {encryptedProfile.GetHashSHA1()} (Length:{encryptedProfile.Length})");
            var ep = EncryptedProfileHelper.Open(encryptedProfile);
            if (ep == null)
            {
                DateWriteLine($"Transmitted Hash: {Array.Empty<byte>().GetHashSHA1()} (Length:0)");
                return;
            }
            var node = EncryptedProfileHelper.DecryptRawAsJson(ep.RawProfile, key);
            if (node == null)
            {
                DateWriteLine($"Transmitted Hash: {Array.Empty<byte>().GetHashSHA1()} (Length:0)");
                return;
            }
            node.Inline = true;
            node["Metadata"] = ep.Metadata.Serialize();
            var result = Encoding.UTF8.GetBytes(node.ToString());
            DateWriteLine($"Transmitted Hash: {result.GetHashSHA1()} (Length:{result.Length})");
            await Response.Body.WriteAsync(result, 0, result.Length);
        }
        [HttpPost("decryptraw/{key}")]
        public async Task DecryptRaw([FromBody] byte[] rawProfile, string key)
        {
            DateWriteLine($"Decrpyt Requested From {HttpContext.GetIpAddress()}");
            DateWriteLine($"Received Hash: {rawProfile.GetHashSHA1()} (Length:{rawProfile.Length})");
            var node = EncryptedProfileHelper.DecryptRawAsJson(rawProfile, key);
            if (node == null)
            {
                DateWriteLine($"Transmitted Hash: {Array.Empty<byte>().GetHashSHA1()} (Length:0)");
                return;
            }
            node.Inline = true;
            var result = Encoding.UTF8.GetBytes(node.ToString());
            DateWriteLine($"Transmitted Hash: {result.GetHashSHA1()} (Length:{result.Length})");
            await Response.Body.WriteAsync(result, 0, result.Length);
        }
        public static void DateWriteLine(string message)
        {
            var dt = DateTime.Now;
            message = $"{dt.Year}/{dt.Month}/{dt.Day} {dt.Hour}:{dt.Minute}:{dt.Second}.{dt.Millisecond} {message}";
            Console.WriteLine(message);
            System.IO.File.AppendAllText("encryptedProfileController.txt", message + '\n');
        }
    }
}