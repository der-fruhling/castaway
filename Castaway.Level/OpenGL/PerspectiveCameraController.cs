using Castaway.Math;

namespace Castaway.Level.OpenGL
{
    public class PerspectiveCameraController : CameraController
    {
        [LevelSerialized("VerticalFOV")] public float FOV;
        
        public override void PreRenderFrame(LevelObject camera, LevelObject? parent)
        {
            base.PreRenderFrame(camera, parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            PerspectiveTransform = CameraMath.Persp(g, g.BoundWindow!.Value, FarCutoff, NearCutoff, MathEx.ToRadians(FOV), Size);
            ViewTransform = Matrix4.Translate(-camera.RealPosition);
        }
    }
}