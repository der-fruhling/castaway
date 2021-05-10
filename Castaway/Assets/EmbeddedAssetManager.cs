using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Castaway.Assets
{
    public class EmbeddedAssetManager : AssetManager
    {
        private readonly Assembly _assembly;

        public EmbeddedAssetManager()
        {
            _assembly = Assembly.GetCallingAssembly();
        }

        public override async Task<uint> LoadAsync(string s)
        {
            return await Task.Run(delegate
            {
                uint slot = 0;
                
                back:
                foreach (var asset in Assets)
                {
                    if (asset.Slot != slot) continue;
                    slot++;
                    goto back;
                }

                IAssetLoader ldr;
                try
                {
                    ldr = AssetLoaders.First(l => l.Matches(s));
                }
                catch (InvalidOperationException)
                {
                    ldr = new StringAsset.Loader();
                }
                
                var path = $"{_assembly.GetName().Name}.{s.Replace('/', '.')}";
                var stream = _assembly.GetManifestResourceStream(path);
                if (stream == null) throw new FileNotFoundException($"No such asset is embedded: {s} (looked for {path})");
                var ary = new byte[stream.Length];
                stream.Read(ary);
                Assets.Add(ldr.Load(slot, ary));
                return slot;
            });
        }
    }
}