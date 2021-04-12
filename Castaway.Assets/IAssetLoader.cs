using System.Collections.Generic;

namespace Castaway.Assets
{
    /// <summary>
    /// Interface class for defining asset loaders.
    /// </summary>
    /// <example><see cref="TextAssetLoader"/></example>
    /// <seealso cref="AssetManager"/>
    public interface IAssetLoader
    {
        /// <summary>
        /// A list of all file extensions to use this loader to process.
        /// </summary>
        public IEnumerable<string> FileExtensions { get; }

        /// <summary>
        /// Loads the file at path.
        /// </summary>
        /// <param name="path">Path of the file to load.</param>
        /// <returns>An object created from the file.</returns>
        public object LoadFile(string path);
    }
}