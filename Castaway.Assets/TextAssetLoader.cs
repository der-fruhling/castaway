using System.Collections.Generic;
using System.IO;

namespace Castaway.Assets
{
    /// <summary>
    /// Loads a string from a text file.
    /// </summary>
    public class TextAssetLoader : IAssetLoader
    {
        public IEnumerable<string> FileExtensions { get; } = new[] {"txt"};
        
        public object LoadFile(string path) => File.ReadAllText(path);
    }
}