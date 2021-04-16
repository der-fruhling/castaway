using Castaway.Assets;
using Castaway.Core;

namespace Castaway.Mesh
{
    /// <summary>
    /// Main module for <c>Castaway.Mesh</c>
    /// </summary>
    public class MeshModule : Module
    {
        protected override void Start()
        {
            base.Start();
            AssetManager.CreateAssetLoader(new STLMesh.Loader());
            AssetManager.CreateAssetLoader(new OBJMesh.Loader());
        }
    }
}