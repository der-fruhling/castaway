using System;
using Castaway.Base;
using Castaway.Level;
using Castaway.Math;
using Castaway.Rendering;
using Serilog;

namespace Castaway.OpenGL.Controllers
{
    [ControllerName("MeshRenderer")]
    [Imports(typeof(OpenGLImpl))]
    public class MeshRendererController : Controller
    {
        private static readonly ILogger Logger = CastawayGlobal.GetLogger();
        private Drawable? _drawable;
        private ShaderObject? _lastBound;

        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            if (parent.Get<MeshController>() == null)
                throw new InvalidOperationException(
                    "MeshRendererController requires a MeshController or one of it's subclasses");
            Logger.Debug("Reading data from {@Controller}", parent.Get<MeshController>());
        }

        public override void OnRender(LevelObject camera, LevelObject parent)
        {
            base.OnRender(camera, parent);
            var g = Graphics.Current;
            g.SetFloatUniform(g.BoundShader!, UniformType.TransformModel,
                Matrix4.Translate(parent.RealPosition) *
                parent.Rotation.ToMatrix4() *
                Matrix4.Scale(parent.Scale));
            if (_lastBound != g.BoundShader || _drawable == null)
            {
                _drawable = parent.Get<MeshController>()!.Mesh!.Value.ConstructFor(g.BoundShader!);
                _lastBound = g.BoundShader;
            }

            g.Draw(g.BoundShader!, _drawable!);
        }
    }
}