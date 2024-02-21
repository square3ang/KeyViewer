using JSON;
using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Utils;
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
        public async Task<byte[]?> Open([FromBody] byte[] encryptedProfile)
        {
            Console.WriteLine("Open Requested");
            return await Task.Run(() =>
            {
                //try
                //{
                return Encoding.UTF8.GetBytes(EncryptedProfileHelper.OpenAsJson(encryptedProfile));
                //}
                //catch { return null; }
            });
        }
        [HttpPost("encrypt/{key}")]
        public async Task<byte[]?> Encrypt([FromBody] byte[] json, string key)
        {
            Console.WriteLine("Encrypt Requested");
            return await Task.Run(() =>
            {
                //try
                //{
                var strJson = Encoding.UTF8.GetString(json);
                var node = JsonNode.Parse(strJson);
                var metadata = ModelUtils.Unbox<Metadata>(node["Metadata"]);
                var profile = ModelUtils.Unbox<Profile>(node["Profile"]);
                return EncryptedProfileHelper.Encrypt(profile, key, metadata);
                //}
                //catch { return null; }
            });
        }
        [HttpPost("decrypt/{key}")]
        public async Task<byte[]?> Decrypt([FromBody] byte[] rawProfile, string key)
        {
            Console.WriteLine("Decrpyt Requested");
            return await Task.Run(() =>
            {
                //try
                //{
                var node = EncryptedProfileHelper.DecryptRaw(rawProfile, key)?.Serialize();
                node!.Inline = true;
                return Encoding.UTF8.GetBytes(node.ToString());
                //}
                //catch { return null; }
            });
        }
        public static string GetHashSHA1(byte[] data) => string.Concat(SHA1.Create().ComputeHash(data).Select(x => x.ToString("X2")));
    }
}