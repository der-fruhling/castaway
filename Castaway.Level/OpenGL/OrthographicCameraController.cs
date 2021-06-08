using Castaway.Math;

namespace Castaway.Level.OpenGL
{
    public class OrthographicCameraController : CameraController
    {
        public override void PreRenderFrame(LevelObject parent)
        {
            base.PreRenderFrame(parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            PerspectiveTransform = CameraMath.Ortho(g, g.BoundWindow!.Value, FarCutoff, NearCutoff, Size);
            ViewTransform = Matrix4.Translate(-parent.RealPosition);
        }
    }
}