using Castaway.Assets;
using Castaway.OpenGL;

namespace Castaway.Level.OpenGL
{
    public class MeshController : EmptyController
    {
        [LevelSerialized("AssetPath")] public Asset? Asset;
        
        public Mesh? Mesh;
        
        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            Mesh = Asset!.Type.To<Mesh>(Asset);
        }
    }
}