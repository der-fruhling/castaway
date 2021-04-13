using Castaway.Core;
using Castaway.Math;
using Castaway.Render;
using Castaway.Window;

namespace Castaway.Level.Controllers.Rendering2D
{
    [ControllerInfo(Name = "Orthographic Camera")]
    public class OrthographicCameraController : Controller
    {
        public readonly uint Id;
        public float FarClip = 100f, NearClip = .01f;
        public float Size = 4f;

        public OrthographicCameraController(uint id)
        {
            Id = id;
        }

        public override void OnBegin()
        {
            base.OnBegin();
            Events.PreDraw += EventPreDraw;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            Events.PreDraw -= EventPreDraw;
        }

        private void EventPreDraw()
        {
            if (level.CurrentCamera != Id) return;
            GLFWWindow.Current.GetWindowSize(out var w, out var h);

            var r = (float) w / h * Size;
            var l = -r;
            var t = Size;
            var b = -Size;

            ShaderManager.ActiveHandle.SetTProjection(new Matrix4(Matrix4.Identity.Array)
            {
                A = 2f / (r - l),
                D = -((r + l) / (r - l)),
                F = 2f / (t - b),
                H = -((t + b) / (t - b)),
                K = -2f / (FarClip - NearClip),
                L = -((FarClip + NearClip) / (FarClip - NearClip)),
            });
            
            ShaderManager.ActiveHandle.SetTView(
                Matrix4.Translate(parent.Position));
        }
    }
}