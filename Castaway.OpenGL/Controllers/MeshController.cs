using Castaway.Assets;
using Castaway.Level;
using Castaway.Rendering.Structures;

namespace Castaway.OpenGL.Controllers
{
    [ControllerName("LoadedMesh")]
    public class MeshController : Controller
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