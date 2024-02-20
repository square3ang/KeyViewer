using JSON;
using Microsoft.AspNetCore.Mvc;
using KeyViewer.WebAPI.Core;

namespace KeyViewer.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguageController : ControllerBase
    {
        public enum Lang
        {
            Korean = 0,
            English = 792770263,
            Chinese = 1604051230,
            Japanese = 234788714,
            Vietnamese = 1191957691
        }
        public static SpreadSheet KTS = new SpreadSheet("1EiWVds23-gZeRCrXL-UYr-o-sc0m-jfqWa-G7qmUYdI");
        public static Dictionary<Lang, string> sheetsJson = new Dictionary<Lang, string>();
        [HttpGet("{lang}")]
        public async Task<string> Get(Lang lang)
        {
            if (!sheetsJson.TryGetValue(lang, out var json))
            {
                var dict = await KTS.Download((int)lang);
                JsonNode node = JsonNode.Empty;
                foreach (var item in dict)
                    node[item.Key] = item.Value;
                sheetsJson[lang] = json = node.ToString();
            }
            return json;
        }
    }
}