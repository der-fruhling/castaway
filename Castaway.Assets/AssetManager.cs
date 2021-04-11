#nullable enable
using System;
using System.Collections.Generic;

namespace Castaway.Assets
{
    public static class AssetManager
    {
        private static List<string?> _names = new List<string?>();
        private static Dictionary<int, object?> _objects = new Dictionary<int, object?>();
        private static List<IAssetLoader> _loaders = new List<IAssetLoader>();
        
        public static int Index(string name)
        {
            var i = _names.IndexOf(name);
            if(i >= 0) return i;
            throw new ApplicationException($"Asset does not exist: {name}");
        }

        private static void CheckIndex(int index)
        {
            if (!_objects.ContainsKey(index) || _names.Count <= index)
                throw new ApplicationException($"Assets: {index} doesn't already exist. Use Create first.");
        }

        public static void Create(string name, object? obj)
        {
            if (_names.Contains(name)) throw new ApplicationException($"Assets: {name} already exists.");
            _names.Add(name);
            _objects[Index(name)] = obj;
        }

        public static void Modify(int index, object value)
        {
            CheckIndex(index);
            _objects[index] = value;
        }

        public static void ModifyName(int index, string name)
        {
            CheckIndex(index);
            _names[index] = name;
        }

        public static T? Get<T>(int index) where T : class
        {
            CheckIndex(index);
            return (T?) _objects[index];
        }

        public static void Erase(int index)
        {
            CheckIndex(index);
            _names[index] = null;
            _objects.Remove(index);
        }

        public static void Modify(string name, object value) => Modify(Index(name), value);
        public static void ModifyName(string name, string value) => ModifyName(Index(name), value);
        public static void Get<T>(int index, out T? o) where T : class => o = Get<T>(index);
        public static void Get<T>(string name, out T? o) where T : class => o = Get<T>(Index(name));
        public static T? Get<T>(string name) where T : class => Get<T>(Index(name));
        public static void Erase(string name) => Erase(Index(name));
        public static void Create(string name) => Create(name, null);

        public static void CreateAssetLoader(IAssetLoader loader) => _loaders.Add(loader);
        public static IAssetLoader[] AssetLoaders => _loaders.ToArray();
    }
}
