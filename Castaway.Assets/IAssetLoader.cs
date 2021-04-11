using System.Collections.Generic;

namespace Castaway.Assets
{
    public interface IAssetLoader
    {
        public IEnumerable<string> FileExtensions { get; }

        public object LoadFile(string path);
    }
}