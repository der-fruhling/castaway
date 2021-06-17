using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.Level.OpenGL
{
    [ControllerName("OrthoCamera")]
    public class OrthographicCameraController : CameraController
    {
        public override void PreRenderFrame(LevelObject camera, LevelObject? parent)
        {
            base.PreRenderFrame(camera, parent);
            var w = Graphics.Current.Window!;
            PerspectiveTransform = CameraMath.Ortho(w, FarCutoff, NearCutoff, Size);
            ViewTransform = Matrix4.Translate(-camera.RealPosition);
        }
    }
}