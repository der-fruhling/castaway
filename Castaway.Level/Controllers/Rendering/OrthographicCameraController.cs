using System.Diagnostics.CodeAnalysis;
using Castaway.Core;
using Castaway.Math;
using Castaway.Native;
using Castaway.Render;
using Castaway.Window;

namespace Castaway.Levels.Controllers.Rendering
{
    [ControllerInfo(Name = "Orthographic Camera")]
    [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class OrthographicCameraController : Controller
    {
        public uint Id;
        public float FarClip = 100f;
        public float NearClip = .01f;
        public float Size = 4f;
        public Vector3 BackgroundColor = Vector3.Zero;
        
        public OrthographicCameraController() {}

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
            GL.ClearColor(new Vector4(BackgroundColor, 1));
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
            
            ShaderManager.ActiveHandle.SetTView(Matrix4.RotateDeg(parent.Rotation) * Matrix4.Translate(parent.Position));

            if (ShaderManager.ActiveHandle.Properties.ContainsKey("ViewPosition"))
            {
                var vname = ShaderManager.ActiveHandle.Properties["ViewPosition"];
                var p = parent.Position;
                ShaderManager.SetUniform(ShaderManager.ActiveHandle, vname, p.X, p.Y, p.Z);
            }
        }
    }
}