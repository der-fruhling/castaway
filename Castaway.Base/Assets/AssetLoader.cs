using System;
using System.Collections.Generic;
using System.IO;

#nullable enable
namespace Castaway.Assets
{
    public class AssetLoader
    {
        public static AssetLoader? Loader;
        
        private Dictionary<string, (string, IAssetType)> Assets;

        public AssetLoader()
        {
            Assets = new Dictionary<string, (string, IAssetType)>();
        }

        public Asset GetAssetByName(string name)
        {
            return new(name, this, Assets[name].Item2);
        }

        internal byte[] GetBytes(Asset asset)
        {
            return File.ReadAllBytes(Assets[asset.Index].Item1);
        }

        private void Discover(string assetPath, string basePath)
        {
            var fullAssetPath = Path.GetFullPath(assetPath);
            var fullBasePath = Path.GetFullPath(basePath);

            foreach (var file in Directory.EnumerateFiles(fullAssetPath))
            {
                var name = file
                    .Remove(file.IndexOf(fullBasePath, StringComparison.Ordinal), fullBasePath.Length)
                    .Replace('\\', '/');
                Assets[name] = (file, GetAssetType(file));
            }

            foreach (var p in Directory.EnumerateDirectories(fullAssetPath)) Discover(p, basePath);
        }

        private static IAssetType GetAssetType(string file)
        {
            return file.Split('.')[^1] switch
            {
                "png" => new ImageAssetType(),
                "jpg" => new ImageAssetType(),
                "jpeg" => new ImageAssetType(),
                "xml" => new XMLAssetType(),
                "obj" => new WavefrontOBJAssetType(),
                _ => new TextAssetType()
            };
        }

        public void Discover(string assetPath)
        {
            Discover(assetPath, assetPath);
        }
    }
}