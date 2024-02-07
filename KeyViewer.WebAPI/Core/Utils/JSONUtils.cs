using JSON;

namespace KeyViewer.WebAPI.Core.Utils
{
    public static class JSONUtils
    {
        public static string? ToStringN(this JsonNode? node)
        {
            if (node == null) return null;
            return node?.Value;
        }
    }
}
