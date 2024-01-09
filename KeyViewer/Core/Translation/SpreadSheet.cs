using JSON;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KeyViewer.Core.Translation
{
    public class SpreadSheet
    {
        private static readonly HttpClient client = new HttpClient();
        private const string URL_START = "https://docs.google.com/spreadsheets/d/";
        private const string URL_END = "/gviz/tq?tqx=out:json&tq&gid=";
        public string UriBase { get; }
        private Dictionary<int, Dictionary<string, string>> dict;
        public SpreadSheet(string key)
        {
            UriBase = URL_START + key + URL_END;
            dict = new Dictionary<int, Dictionary<string, string>>();
        }
        public string this[int gid, string key]
        {
            get
            {
                if (dict.TryGetValue(gid, out var kv) &&
                    kv.TryGetValue(key, out var result))
                    return result;
                return key;
            }
            set
            {
                if (dict.TryGetValue(gid, out var kv))
                    kv[key] = value;
            }
        }
        public Dictionary<string, string> Get(int gid)
        {
            return dict.TryGetValue(gid, out var kv) ? kv : null;
        }
        public async Task<Dictionary<string, string>> Download(int gid, Action<Dictionary<string, string>> onDownloaded = null, bool force = false)
        {
            var exist = Get(gid);
            if (!force && exist != null)
            {
                onDownloaded?.Invoke(exist);
                return exist;
            }
            string uri = UriBase + gid;
            var gidDict = dict[gid] = new Dictionary<string, string>();
            byte[] bytes = await client.GetByteArrayAsync(uri);
            string strData = Encoding.UTF8.GetString(bytes);
            strData = strData.Substring(47, strData.Length - 49);
            JsonNode data = JsonNode.Parse(strData);
            JsonNode rows = data["table"]["rows"];
            foreach (JsonNode row in rows)
            {
                JsonNode keyValue = row["c"];
                string key = keyValue[0]["v"];
                string value = keyValue[1]["v"];
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                    continue;
                gidDict.Add(key, value);
            }
            onDownloaded?.Invoke(gidDict);
            return gidDict;
        }
    }
}
