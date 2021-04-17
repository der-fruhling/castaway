using System;
using System.Diagnostics.CodeAnalysis;
using Castaway.Core;
using Castaway.Math;
using Castaway.Native;
using Castaway.Render;
using Castaway.Window;

namespace Castaway.Levels.Controllers.Rendering
{
    [ControllerInfo(Name = "Perspective Camera")]
    [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PerspectiveCameraController : Controller
    {
        public uint Id;
        public float FarClip = 100f;
        public float NearClip = .01f;
        public float Size = 1f;
        public float FOV = 60f;
        public Vector3 BackgroundColor = Vector3.Zero;

        public PerspectiveCameraController() {}
        
        public PerspectiveCameraController(uint id)
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

            var a = (float) w / h * Size;
            var b = Size;
            var c = MathF.Tan(CMath.Radians(FOV / 2f));

            ShaderManager.ActiveHandle.SetTProjection(new Matrix4(Matrix4.Identity.Array)
            {
                A = 1f / (a * c),
                F = 1f / (b * c),
                K = (-NearClip - FarClip) / (NearClip - FarClip),
                L = 2 * FarClip * NearClip / (NearClip - FarClip),
                O = 1,
                P = 0
            });
            
            ShaderManager.ActiveHandle.SetTView(Matrix4.RotateDeg(parent.Rotation) * Matrix4.Translate(parent.Position));
            
            if (ShaderManager.ActiveHandle.Properties.ContainsKey("ViewPosition"))
            {
                var vname = ShaderManager.ActiveHandle.Properties["ViewPosition"];
                ShaderManager.ActiveHandle.SetUniform(vname, parent.Position);
            }
        }
    }
}