using System;
using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.Level.OpenGL
{
    public class MeshRendererController : EmptyController
    {
        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            if (parent.Get<MeshController>() == null)
                throw new InvalidOperationException(
                    "MeshRendererController requires a MeshController or one of it's subclasses");
        }

        public override void OnRender(LevelObject camera, LevelObject parent)
        {
            base.OnRender(camera, parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            var d = parent.Get<MeshController>()!.Mesh!.Value.ConstructFor(g, g.BoundProgram!.Value);
            g.SetUniform(g.BoundProgram!.Value, UniformType.TransformModel,
                Matrix4.Translate(parent.RealPosition) *
                Matrix4.Scale(parent.Scale));
            g.Draw(g.BoundProgram!.Value, d);
            g.Destroy(d.ElementArray!.Value, d.VertexArray!.Value);
        }
    }
}