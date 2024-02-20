using JSON;
using KeyViewer.Models;
using KeyViewer.Utils;
using KeyViewer.WebAPI.Core;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace KeyViewer.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EncryptedProfileController : ControllerBase
    {
        [HttpPost("open")]
        public async Task<byte[]?> Open(byte[] encryptedProfile)
        {
            Console.WriteLine("Open Requested");
            return await Task.Run(() =>
            {
                //try
                //{
                return Encoding.UTF8.GetBytes(EP.OpenEncryptedProfileAsJson(encryptedProfile));
                //}
                //catch { return null; }
            });
        }
        [HttpPost("encrypt/{key}")]
        public async Task<byte[]?> Encrypt(byte[] json, string key)
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
                return EP.EncryptProfile(profile, key, metadata);
                //}
                //catch { return null; }
            });
        }
        [HttpPost("decrypt/{key}")]
        public async Task<byte[]?> Decrypt(byte[] rawProfile, string key)
        {
            Console.WriteLine("Decrpyt Requested");
            return await Task.Run(() =>
            {
                //try
                //{
                var node = EP.DecryptRawProfile(rawProfile, key)?.Serialize();
                node!.Inline = true;
                return Encoding.UTF8.GetBytes(node.ToString());
                //}
                //catch { return null; }
            });
        }
    }
}