using Castaway.Assets;
using Castaway.Core;

namespace PirateSLC
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