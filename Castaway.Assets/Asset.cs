using System;

#nullable enable
namespace Castaway.Assets
{
    public class Asset
    {
        private readonly AssetLoader _loader;
        public readonly string Index;
        private byte[]? _bytes;
        private bool _isLoaded;
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
            if (_isLoaded) return;
            _isLoaded = true;
            _bytes = _loader.GetBytes(this);
        }

        public void Unload()
        {
            if (!_isLoaded) return;
            _isLoaded = false;
            _bytes = null;
        }

        public byte[] GetBytes()
        {
            return _bytes ?? throw new InvalidOperationException("Not loaded.");
        }

        public T To<T>()
        {
            return Type.To<T>(this);
        }
    }
}