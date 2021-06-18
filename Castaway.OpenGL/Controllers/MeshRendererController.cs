using System;
using Castaway.Level;
using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.OpenGL.Controllers
{
    [ControllerName("MeshRenderer")]
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
            var g = Graphics.Current;
            using var d = parent.Get<MeshController>()!.Mesh!.Value.ConstructFor(g.BoundShader!);
            g.SetFloatUniform(g.BoundShader!, UniformType.TransformModel,
                Matrix4.Translate(parent.RealPosition) *
                parent.Rotation.ToMatrix4() *
                Matrix4.Scale(parent.Scale));
            g.Draw(g.BoundShader!, d);
        }
    }
}