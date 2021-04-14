using System;
using System.Diagnostics.CodeAnalysis;
using Castaway.Core;
using Castaway.Math;
using Castaway.Render;
using Castaway.Window;

namespace Castaway.Level.Controllers.Rendering
{
    [ControllerInfo(Name = "Perspective Camera")]
    [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PerspectiveCameraController : Controller
    {
        public readonly uint Id;
        public float FarClip = 100f;
        public float NearClip = .01f;
        public float Size = 1f;
        public float FOV = 60f;

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
            GLFWWindow.Current.GetWindowSize(out var w, out var h);

            var a = (float) w / h * Size;
            var b = Size;
            var c = MathF.Tan(CMath.Radians(FOV / 2f));

            ShaderManager.ActiveHandle.SetTProjection(new Matrix4(Matrix4.Identity.Array)
            {
                A = 1f / (a * c),
                F = 1f / c,
                K = (-NearClip - FarClip) / (NearClip - FarClip),
                L = 2 * FarClip * NearClip / (NearClip - FarClip),
                O = 1,
                P = 0
            });
            
            ShaderManager.ActiveHandle.SetTView(
                Matrix4.Translate(parent.Position) * Matrix4.Rotate(parent.Rotation));
        }
    }
}