using JSON;
using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Utils;
using KeyViewer.WebAPI.Core;
using KeyViewer.WebAPI.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
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
            Console.WriteLine($"Open Requested From {HttpContext.GetIpAddress()}");
            Console.WriteLine($"Received Hash: {encryptedProfile.GetHashSHA1()} (Length:{encryptedProfile.Length})");
            var json = EncryptedProfileHelper.OpenAsJson(encryptedProfile)?.ToString();
            if (json == null) return;
            var result = Encoding.UTF8.GetBytes(json);
            Console.WriteLine($"Transmitted Hash: {result.GetHashSHA1()} (Length:{result.Length})");
            await Response.Body.WriteAsync(result, 0, result.Length);
        }
        [HttpPost("encrypt/{key}")]
        public async Task Encrypt([FromBody] byte[] json, string key)
        {
            Console.WriteLine($"Encrypt Requested From {HttpContext.GetIpAddress()}");
            Console.WriteLine($"Received Hash: {json.GetHashSHA1()} (Length:{json.Length})");
            var strJson = Encoding.UTF8.GetString(json);
            var node = JsonNode.Parse(strJson);
            var metadata = ModelUtils.Unbox<Metadata>(node["Metadata"]);
            var profileNode = node["Profile"];
            var references = ModelUtils.UnwrapList<ProfileImporter.Reference>((JsonArray)profileNode["References"]);
            var profile = ModelUtils.Unbox<Profile>(profileNode);
            var result = EncryptedProfileHelper.Encrypt(profile, key, metadata, references);
            if (result == null) return;
            Console.WriteLine($"Transmitted Hash: {result.GetHashSHA1()} (Length:{result.Length})");
            await Response.Body.WriteAsync(result, 0, result.Length);
        }
        [HttpPost("decrypt/{key}")]
        public async Task Decrypt([FromBody] byte[] rawProfile, string key)
        {
            Console.WriteLine($"Decrpyt Requested From {HttpContext.GetIpAddress()}");
            Console.WriteLine($"Received Hash: {rawProfile.GetHashSHA1()} (Length:{rawProfile.Length})");
            var node = EncryptedProfileHelper.DecryptRawAsJson(rawProfile, key);
            if (node == null) return;
            node.Inline = true;
            var result = Encoding.UTF8.GetBytes(node.ToString());
            Console.WriteLine($"Transmitted Hash: {result.GetHashSHA1()} (Length:{result.Length})");
            await Response.Body.WriteAsync(result, 0, result.Length);
        }
        public static string GetHashSHA1(byte[] data) => string.Concat(SHA1.Create().ComputeHash(data).Select(x => x.ToString("X2")));
    }
}