using System;

#nullable enable
namespace Castaway.Assets
{
    public class Asset
    {
        private readonly AssetLoader _loader;
        internal readonly string Index;
        private bool _isLoaded;
        private byte[]? _bytes;
        public IAssetType Type;

        internal Asset(string index, AssetLoader loader, IAssetType type)
        {
            Index = index;
            _loader = loader;
            Type = type;
            Load();
        }

        public void Load()
        {
            if(_isLoaded) return;
            _isLoaded = true;
            _bytes = _loader.GetBytes(this);
        }

        public void Unload()
        {
            if(!_isLoaded) return;
            _isLoaded = false;
            _bytes = null;
        }

        public byte[] GetBytes() => _bytes ?? throw new InvalidOperationException("Not loaded.");
    }
}