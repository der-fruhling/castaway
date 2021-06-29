using Castaway.Assets;
using Castaway.Base;
using Castaway.OpenGL;
using Castaway.Rendering.Structures;

namespace Castaway.Level.OpenGL
{
    [ControllerName("LoadedMesh")]
    [Imports(typeof(OpenGLImpl))]
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