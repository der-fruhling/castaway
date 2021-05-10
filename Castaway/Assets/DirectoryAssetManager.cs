using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Castaway.Assets
{
    public class DirectoryAssetManager : AssetManager
    {
        private string _dir;
        private Mutex _mutex = new();

        public DirectoryAssetManager(string dir)
        {
            _dir = Path.GetFullPath(dir);
        }

        public override async Task<uint> LoadAsync(string s)
        {
            return await Task.Run(delegate
            {
                uint slot = 0;

                var ldr = AssetLoaders.First(l => l.Matches(s));

                _mutex.WaitOne();
                back:
                foreach (var asset in Assets)
                {
                    if (asset.Slot != slot) break;
                    slot++;
                    goto back;
                }
                
                Assets.Add(ldr.Load(slot, File.ReadAllBytes($"{_dir}/{s}")));
                _mutex.ReleaseMutex();
                return slot;
            });
        }
    }
}