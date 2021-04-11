using System.Collections.Generic;
using System.IO;

namespace Castaway.Assets
{
    public class TextAssetLoader : IAssetLoader
    {
        public IEnumerable<string> FileExtensions { get; } = new[] {"txt"};
        
        public object LoadFile(string path) => File.ReadAllText(path);
    }
}