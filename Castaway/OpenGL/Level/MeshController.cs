using Castaway.Assets;
using Castaway.Level;
using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.OpenGL.Level
{
    public class MeshController : EmptyController
    {
        [LevelSerialized("AssetPath")]
        public Asset? Asset;
        
        public Mesh? Mesh;
        private IDrawable? _drawable;
        
        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            var g = OpenGL.Get();
            Mesh = Asset!.Type.To<Mesh>(Asset);
            _drawable = Mesh.Value.ConstructFor(g, g.BoundProgram!.Value);
        }

        public override void OnRender(LevelObject parent)
        {
            base.OnRender(parent);
            var g = OpenGL.Get();
            g.SetUniform(g.BoundProgram!.Value, UniformType.TransformModel,
                Matrix4.Translate(parent.RealPosition) *
                Matrix4.Scale(parent.Scale));
            g.Draw(g.BoundProgram!.Value, _drawable!);
        }
    }
}