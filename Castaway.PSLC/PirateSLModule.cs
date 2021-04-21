using Castaway.Assets;
using Castaway.Core;

namespace Castaway.PSLC
{
    // ReSharper disable once InconsistentNaming
    public class PirateSLModule : Module
    {
        protected override void Start()
        {
            base.Start();
            AssetManager.CreateAssetLoader(new PirateSLAssetLoader());
        }
    }
}