using Castaway.Assets;
using Castaway.Structures;

namespace Castaway.Level.OpenGL
{
    [ControllerName("LoadedMesh")]
    public class MeshController : EmptyController
    {
        [LevelSerialized("AssetPath")] public Asset? Asset;
        
        public Mesh? Mesh;
        
        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            Mesh = Asset!.Type.To<Mesh>(Asset);
        }
    }
}