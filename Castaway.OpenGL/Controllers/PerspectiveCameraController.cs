using Castaway.Level;
using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.OpenGL.Controllers
{
    [ControllerName("PerspCamera")]
    public class PerspectiveCameraController : CameraController
    {
        [LevelSerialized("VerticalFOV")] public float FOV;
        
        public override void PreRenderFrame(LevelObject camera, LevelObject? parent)
        {
            base.PreRenderFrame(camera, parent);
            var w = Graphics.Current.Window!;
            PerspectiveTransform = CameraMath.Persp(w, FarCutoff, NearCutoff, MathEx.ToRadians(FOV), Size);
            ViewTransform = camera.Rotation.Normalize().Conjugate().ToMatrix4()
                            * Matrix4.Translate(-camera.RealPosition);
        }
    }
}