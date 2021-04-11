using System;
using System.IO;
using System.Linq;
using Castaway.Core;

namespace Castaway.Assets
{
    public class AssetsModule : Module
    {
        protected override void Start()
        {
            base.Start();
            AssetManager.CreateAssetLoader(new TextAssetLoader());
        }

        protected override void PreInit()
        {
            base.PreInit();
            var assetFolderPath = "Assets";

            var gameFolder = Environment.GetEnvironmentVariable("GAME_FOLDER");
            if (gameFolder != null)
                assetFolderPath = $"{gameFolder}/Assets";

            EnumerateFolder(AssetManager.AssetLoaders, assetFolderPath, assetFolderPath);
        }

        private static void EnumerateFolder(IAssetLoader[] loaders, string path, string assetPath)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var assetName = file.Replace(assetPath, "");
                foreach (var assetLoader in loaders)
                {
                    if (!assetLoader.FileExtensions.Any(s => file.EndsWith($".{s}"))) continue;
                    var o = assetLoader.LoadFile(file);
                    AssetManager.Create(assetName, o);
                    goto loaderLoopExit;
                }
                
                AssetManager.Create(assetName, File.ReadAllBytes(file));
                
                loaderLoopExit: ;
            }

            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs) EnumerateFolder(loaders, dir, assetPath);
        }
    }
}