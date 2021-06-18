using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Castaway.Assets
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class LoadsAttribute : Attribute
    {
        public string[] Extensions;

        public LoadsAttribute(params string[] extensions)
        {
            Extensions = extensions;
        }
    }
    
    public static class AssetLoaderFinder
    {
        private static Dictionary<string, Type> _loaders;
        
        static AssetLoaderFinder()
        {
            _loaders = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Contains(typeof(IAssetType))
                            && t.GetCustomAttribute<LoadsAttribute>() != null)
                .Select(t => (t.GetCustomAttribute<LoadsAttribute>()!.Extensions, t))
                .SelectMany(t => t.Extensions.Select(e => (e, t.t)))
                .ToDictionary(t => t.e, t => t.t);
        }

        public static IAssetType Get(string extension) => (_loaders.ContainsKey(extension)
            ? Activator.CreateInstance(_loaders[extension]) as IAssetType
            : new TextAssetType()) ?? throw new InvalidOperationException($"Bad asset loader for {extension}");
    }
}