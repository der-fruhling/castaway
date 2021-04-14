using Castaway.Assets;
using Castaway.Core;

namespace Castaway.Serializable
{
    public class SerializableModule : Module
    {
        protected override void Start()
        {
            base.Start();
            AssetManager.CreateAssetLoader(new LevelAssetLoader());
        }
    }
}