using System;
using Castaway.Math;
using Castaway.OpenGL;
using Castaway.Rendering;

namespace Castaway.Level.OpenGL
{
    public class MeshRendererController : EmptyController
    {
        private IDrawable _drawable;
        
        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            if (parent.Get<MeshController>() == null)
                throw new InvalidOperationException(
                    "MeshRendererController requires a MeshController or one of it's subclasses");
            var g = Castaway.OpenGL.OpenGL.Get();
            _drawable = parent.Get<MeshController>()!.Mesh!.Value.ConstructFor(g, g.BoundProgram!.Value);
        }

        public override void OnRender(LevelObject parent)
        {
            base.OnRender(parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            g.SetUniform(g.BoundProgram!.Value, UniformType.TransformModel,
                Matrix4.Translate(parent.RealPosition) *
                Matrix4.Scale(parent.Scale));
            g.Draw(g.BoundProgram!.Value, _drawable!);
        }
    }
}