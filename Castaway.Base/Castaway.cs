using System.IO;
using System.Text.Json;
using Castaway.Assets;

namespace Castaway
{
    public static class CastawayEngine
    {
        public static void Init()
        {
            var doc = JsonDocument.Parse(File.ReadAllText("config.json"));
            var root = doc.RootElement;
            AssetLoader.Loader = new AssetLoader();
            if (!root.TryGetProperty("assets", out var assetsElement)) return;
            if (!assetsElement.TryGetProperty("discover", out var discoverElement)) return;
            foreach(var s in discoverElement.EnumerateArray())
                AssetLoader.Loader.Discover(s.GetString()!);
        }
    }
}