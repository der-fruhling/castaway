using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castaway.Assets
{
    public interface IAssetLoader
    {
        public bool Matches(string filename);
        public Asset Load(uint slot, byte[] bytes);
    }
    
    public class Asset
    {
        public readonly uint Slot;

        protected Asset(uint slot)
        {
            Slot = slot;
        }
    }
    
    public abstract class AssetManager
    {
        protected ConcurrentBag<Asset> Assets = new();
        protected ConcurrentBag<IAssetLoader> AssetLoaders = new();

        protected AssetManager()
        {
            AddAssetLoader(new StringAsset.Loader());
        }
        
        public void AddAssetLoader(IAssetLoader loader) => AssetLoaders.Add(loader);

        public abstract Task<uint> LoadAsync(string s);

        public async Task<T> GetAsync<T>(uint r) where T : Asset =>
            await Task.Run(() => Assets.First(a => a.Slot == r) as T);

        public uint Load(string s) => LoadAsync(s).Result;
        public T Get<T>(uint r) where T : Asset => GetAsync<T>(r).Result;
    }

    public class StringAsset : Asset
    {
        public readonly string Value;
        
        private StringAsset(uint slot, string value) : base(slot)
        {
            Value = value;
        }

        public static implicit operator string(StringAsset a) => a.Value;

        internal class Loader : IAssetLoader
        {
            public bool Matches(string filename) => true;
            public Asset Load(uint slot, byte[] bytes) => new StringAsset(slot, Encoding.UTF8.GetString(bytes));
        }
    }
}