#nullable enable
using System;
using System.Collections.Generic;

namespace Castaway.Assets
{
    /// <summary>
    /// Allows accessing assets.
    /// </summary>
    public static class AssetManager
    {
        private static List<string?> _names = new List<string?>();
        private static Dictionary<int, object?> _objects = new Dictionary<int, object?>();
        private static List<IAssetLoader> _loaders = new List<IAssetLoader>();

        public const string AssetFolderPath = "Assets";
        
        /// <summary>
        /// Gets the index of an asset by name.
        /// </summary>
        /// <param name="name">Name of the asset to search for.</param>
        /// <returns>Index of the asset.</returns>
        public static int Index(string name)
        {
            return _names.IndexOf(name);
        }

        /// <summary>
        /// Checks if an asset completely exists.
        /// </summary>
        /// <param name="index">Index of the asset to check.</param>
        /// <exception cref="ApplicationException">Thrown if the asset does
        /// not exist.</exception>
        private static void CheckIndex(int index)
        {
            if (!_objects.ContainsKey(index) || _names.Count <= index)
                throw new ApplicationException($"Assets: {index} doesn't already exist. Use Create first.");
        }

        /// <summary>
        /// Creates a new asset with the specified name and contents.
        /// </summary>
        /// <param name="name">Name of the asset.</param>
        /// <param name="obj">Contents of the asset.</param>
        /// <exception cref="ApplicationException">Thrown if the asset already
        /// exists.</exception>
        public static void Create(string name, object? obj)
        {
            if (_names.Contains(name)) throw new ApplicationException($"Assets: {name} already exists.");
            _names.Add(name);
            _objects[Index(name)] = obj;
        }

        /// <summary>
        /// Changes the contents of an already existing asset. The asset
        /// must exist.
        /// </summary>
        /// <param name="index">Index of the asset to modify.</param>
        /// <param name="value">New value.</param>
        /// <exception cref="ApplicationException">Thrown by
        /// <see cref="CheckIndex"/></exception>
        public static void Modify(int index, object? value)
        {
            CheckIndex(index);
            _objects[index] = value;
        }

        /// <summary>
        /// Changes the name of an asset. The asset must exist.
        /// </summary>
        /// <param name="index">Index of the asset to rename.</param>
        /// <param name="name">New name of the asset.</param>
        /// <exception cref="ApplicationException">Thrown by
        /// <see cref="CheckIndex"/></exception>
        public static void ModifyName(int index, string name)
        {
            CheckIndex(index);
            _names[index] = name;
        }

        /// <summary>
        /// Reads an asset's contents by index, casting it the specified type.
        /// The asset must exist.
        /// </summary>
        /// <param name="index">Index of the asset to read.</param>
        /// <typeparam name="T">Type to cast to.</typeparam>
        /// <returns>Contents of the asset.</returns>
        /// <exception cref="ApplicationException">Thrown by
        /// <see cref="CheckIndex"/></exception>
        /// <exception cref="ApplicationException">Thrown if the type was
        /// incorrect, never thrown if the value is <c>null</c>.</exception>
        public static T? Get<T>(int index) where T : class
        {
            CheckIndex(index);
            var o = _objects[index];
            if (o != null && o.GetType() != typeof(T))
                throw new ApplicationException($"Invalid object type: {o.GetType()} != {typeof(T)}");
            return (T?) o;
        }

        /// <summary>
        /// Deletes an asset. The asset must exist.
        /// </summary>
        /// <param name="index">Index of the asset to delete.</param>
        /// <exception cref="ApplicationException">Thrown by
        /// <see cref="CheckIndex"/></exception>
        public static void Erase(int index)
        {
            CheckIndex(index);
            _names[index] = null;
            _objects.Remove(index);
        }

        /// <seealso cref="Modify(int,object?)"/>
        [Obsolete] public static void Modify(string name, object value) => Modify(Index(name), value);
        /// <seealso cref="ModifyName(int,string)"/>
        [Obsolete] public static void ModifyName(string name, string value) => ModifyName(Index(name), value);
        /// <seealso cref="Get{T}(int)"/>
        public static void Get<T>(int index, out T? o) where T : class => o = Get<T>(index);
        /// <seealso cref="Get{T}(int)"/>
        [Obsolete] public static void Get<T>(string name, out T? o) where T : class => o = Get<T>(Index(name));
        /// <seealso cref="Get{T}(int)"/>
        [Obsolete] public static T? Get<T>(string name) where T : class => Get<T>(Index(name));
        /// <seealso cref="Erase(int)"/>
        [Obsolete] public static void Erase(string name) => Erase(Index(name));
        /// <seealso cref="Create(string,object?)"/>
        [Obsolete] public static void Create(string name) => Create(name, null);

        /// <summary>
        /// Adds an asset loader that can be used to convert assets from files
        /// to objects.
        /// </summary>
        /// <param name="loader">Asset loader to add</param>
        /// <seealso cref="AssetsModule"/>
        /// <seealso cref="IAssetLoader"/>
        public static void CreateAssetLoader(IAssetLoader loader) => _loaders.Add(loader);
        internal static IAssetLoader[] AssetLoaders => _loaders.ToArray();
    }
}
